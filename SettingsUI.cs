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

            input.Text = PanelUI.Settings.GhostKey + "";
        }

        public SettingsUI()
        {
            InitializeComponent();

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

            Label title = new Label() { Text = "Settings", Font = new Font("Bahnschrift SemiBold", 20F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))), Top = 10, Height = 40, Width = 130 };
            title.Left = (this.Width - title.Width) / 2;
            this.Controls.Add(title);

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

            this.ghostShiftCheck = new CheckBox() { Text = "Shift", Top = 50, Left = 10, Width = 50 };
            this.ghostCtrlCheck = new CheckBox() { Text = "Ctrl", Top = 50, Left = 70, Width = 50 };
            this.ghostAltCheck = new CheckBox() { Text = "Alt", Top = 50, Left = 120, Width = 40 };

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
        }
    }
}
