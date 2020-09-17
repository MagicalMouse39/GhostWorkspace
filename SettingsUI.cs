using GhostWorkspace.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GhostWorkspace
{
    public partial class SettingsUI : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private bool waitingForKey = false;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private CheckBox ghostShiftCheck, ghostCtrlCheck, ghostAltCheck;

        private void AnimatePopup(bool visible)
        {   
            Task.Factory.StartNew(() =>
            {
                Rectangle screen = Screen.PrimaryScreen.Bounds;
                for (int i = visible ? 0 : 512; visible ? i < 512 : i > 0; i += visible ? 1 : -1)
                {
                    try
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            this.Height = i;
                            this.Width = i;
                            this.Left = (screen.Width - this.Width) / 2;
                            this.Top = (screen.Height - this.Height) / 2;
                        }));
                    }
                    catch (Exception ex) { break; }

                    if (i % 3 == 0)
                        Thread.Sleep(1);
                }
                try
                {
                    if (!visible)
                        this.BeginInvoke(new Action(() => this.Visible = false));
                }
                catch { }
            });
        }

        private void UpdateKeyModifiers()
        {
            this.ghostShiftCheck.Checked = PanelUI.Settings.GhostShift;
            this.ghostCtrlCheck.Checked = PanelUI.Settings.GhostCtrl;
            this.ghostAltCheck.Checked = PanelUI.Settings.GhostAlt;
        }

        private void CheckForKey(KeyEventArgs e, Button input)
        {
            if (!this.waitingForKey)
                return;

            PanelUI.Settings.GhostKey = (char)e.KeyValue;
            PanelUI.Settings.GhostCtrl = e.Control;
            PanelUI.Settings.GhostAlt = e.Alt;
            PanelUI.Settings.GhostShift = e.Shift;

            this.UpdateKeyModifiers();

            PanelUI.SaveChanges();

            input.Text = PanelUI.Settings.GhostKey + "";
        }

        private void AddComponents()
        {
            Label title = new Label() { Text = "Settings", Font = new Font("Bahnschrift SemiBold", 20F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))), Top = 10, Height = 40, Width = 130 };
            title.Left = (this.Width - title.Width) / 2;
            this.Controls.Add(title);

            Button closeBtn = new Button() { Top = 3, BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Resources.CloseBtnIcon, Left = this.Width - 33, Width = 30, Height = 30, FlatStyle = FlatStyle.Flat, ForeColor = PanelUI.Settings.SettingsBG };
            closeBtn.Click += (s, e) => this.Close();
            this.Controls.Add(closeBtn);

            GroupBox ghostKeyBox = new GroupBox() { Text = "Ghosting Key Combo", Top = 60, Width = 170 };
            ghostKeyBox.Left = (this.Width - ghostKeyBox.Width) / 2;

            Label ghostKeyLabel = new Label() { Text = "Key:", Top = 22, Left = 10, Width = 30 };
            ghostKeyBox.Controls.Add(ghostKeyLabel);
            Button ghostKeyInput = new Button() { Text = PanelUI.Settings.GhostKey + "", Top = 20, Left = 60 };
            ghostKeyInput.Click += (s, e) =>
            {
                if (this.waitingForKey)
                    return;

                this.waitingForKey = true;

                ghostKeyInput.Text = "...";
            };
            ghostKeyBox.Controls.Add(ghostKeyInput);

            this.ghostShiftCheck = new CheckBox() { Text = "Shift", Top = 50, Left = 10, Width = 50, Checked = PanelUI.Settings.GhostShift };
            this.ghostCtrlCheck = new CheckBox() { Text = "Ctrl", Top = 50, Left = 70, Width = 50, Checked = PanelUI.Settings.GhostCtrl };
            this.ghostAltCheck = new CheckBox() { Text = "Alt", Top = 50, Left = 120, Width = 40, Checked = PanelUI.Settings.GhostAlt };

            this.ghostShiftCheck.CheckedChanged += (s, e) =>
            {
                PanelUI.Settings.GhostShift = this.ghostShiftCheck.Checked;
                PanelUI.SaveChanges();
            };
            
            this.ghostCtrlCheck.CheckedChanged += (s, e) =>
            {
                PanelUI.Settings.GhostCtrl = this.ghostCtrlCheck.Checked;
                PanelUI.SaveChanges();
            };

            this.ghostAltCheck.CheckedChanged += (s, e) =>
            {
                PanelUI.Settings.GhostAlt = this.ghostAltCheck.Checked;
                PanelUI.SaveChanges();
            };

            ghostKeyBox.Controls.Add(this.ghostShiftCheck);
            ghostKeyBox.Controls.Add(this.ghostCtrlCheck);
            ghostKeyBox.Controls.Add(this.ghostAltCheck);

            this.Controls.Add(ghostKeyBox);

            ghostKeyInput.KeyDown += (s, e) => CheckForKey(e, ghostKeyInput);
            ghostKeyBox.KeyDown += (s, e) => CheckForKey(e, ghostKeyInput);
            this.KeyDown += (s, e) => CheckForKey(e, ghostKeyInput);

            title.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            Button sidePanelBGBtn = new Button() { Text = "Side Panel BG", BackColor = PanelUI.Settings.SidePanelBG, Width = 170, Top = 180, Left = (this.Width - 170) / 2, FlatStyle = FlatStyle.Flat, Height = 30 };
            sidePanelBGBtn.Click += (s, e) =>
            {
                var cd = new ColorDialog() { Color = PanelUI.Settings.SidePanelBG };

                if (cd.ShowDialog() == DialogResult.OK)
                {
                    PanelUI.Settings.SidePanelBG = cd.Color;
                    PanelUI.SaveChanges();
                    sidePanelBGBtn.BackColor = cd.Color;
                    PanelUI.instance.UpdateColors();
                }
            };

            TrackBar sidePanelAlphaBar = new TrackBar() { Width = 170, Maximum = 255, Minimum = 1, Left = (this.Width - 170) / 2, Top = 220, Value = (int)(PanelUI.Settings.SidePanelAlpha * 255) };
            sidePanelAlphaBar.ValueChanged += (s, e) =>
            {
                PanelUI.Settings.SidePanelAlpha = (sidePanelAlphaBar.Value / 255f);
                PanelUI.instance.UpdateColors();
                PanelUI.SaveChanges();
            };
            this.Controls.Add(sidePanelAlphaBar);

            this.Controls.Add(sidePanelBGBtn);

            Button settingsBGBtn = new Button() { Text = "Settings BG", BackColor = PanelUI.Settings.SettingsBG, Width = 170, Top = 270, Left = (this.Width - 170) / 2, FlatStyle = FlatStyle.Flat, Height = 30 };
            settingsBGBtn.Click += (s, e) =>
            {
                var cd = new ColorDialog() { Color = PanelUI.Settings.SettingsBG };

                if (cd.ShowDialog() == DialogResult.OK)
                {
                    PanelUI.Settings.SettingsBG = cd.Color;
                    PanelUI.SaveChanges();
                    settingsBGBtn.BackColor = cd.Color;
                    this.BackColor = cd.Color;
                }
            };

            TrackBar settingsAlphaBar = new TrackBar() { Width = 170, Maximum = 255, Minimum = 1, Left = (this.Width - 170) / 2, Top = 310, Value = (int)(PanelUI.Settings.SettingsAlpha * 255) };
            settingsAlphaBar.ValueChanged += (s, e) =>
            {
                PanelUI.Settings.SettingsAlpha = (settingsAlphaBar.Value / 255f);
                this.Opacity = PanelUI.Settings.SettingsAlpha;
                PanelUI.SaveChanges();
            };
            this.Controls.Add(settingsAlphaBar);

            this.Controls.Add(settingsBGBtn);

            CheckBox killProcsAtExit = new CheckBox() { Text = "Kill processes at Exit", Width = 125, Left = (this.Width - 120) / 2, Top = 350, Checked = PanelUI.Settings.CloseProcessesAtExit };
            killProcsAtExit.CheckedChanged += (s, e) =>
            {
                PanelUI.Settings.CloseProcessesAtExit = killProcsAtExit.Checked;
                PanelUI.SaveChanges();
            };
            this.Controls.Add(killProcsAtExit);
        }

        public SettingsUI()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.StartPosition = FormStartPosition.CenterScreen;

            this.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                this.AnimatePopup(false);
            };

            this.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                    this.AnimatePopup(true);
            };

            this.FormBorderStyle = FormBorderStyle.None;
            this.Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));

            this.BackColor = PanelUI.Settings.SettingsBG;
            this.Opacity = PanelUI.Settings.SettingsAlpha;

            this.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            this.AddComponents();
        }
    }
}
