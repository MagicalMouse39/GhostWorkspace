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
    public partial class AddAppUI : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void AddComponents()
        {
            this.Controls.Clear();
            Label title = new Label() { Text = "Add an App", Font = new Font("Bahnschrift SemiBold", 20F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))), Top = 10, Height = 40, Width = 180 };
            title.Left = (this.Width - title.Width) / 2;
            title.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };
            this.Controls.Add(title);

            Button selectAppBtn = new Button() { Text = "Select an Application", Width = this.Width / 2, FlatStyle = FlatStyle.Flat, Left = this.Width / 4, Height = this.Height / 5, Top = (this.Height - this.Height / 5) / 2, Font = new Font("Bahnschrift SemiBold", 15F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))) };
            selectAppBtn.Click += (s, e) =>
            {
                var ofd = new OpenFileDialog() { Title = "Select an Application" };

                if (ofd.ShowDialog() == DialogResult.OK)
                    PanelUI.instance.AddApplication(ofd.FileName, true);

                PanelUI.SaveChanges();
            };

            Button okBtn = new Button() { Text = "OK", Left = (this.Width - 100) / 2, Width = 100, Height = 30, Top = (this.Height - this.Height / 5) / 2 + 200, FlatStyle = FlatStyle.Flat, Font = new Font("Bahnschrift SemiBold", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))) };
            okBtn.Click += (s, e) => this.AnimatePopup(false);

            this.Controls.Add(okBtn);
            this.Controls.Add(selectAppBtn);
        }

        private void RemoveComponents() => this.Controls.Clear();

        private void AnimatePopup(bool visible)
        {
            if (!visible)
                this.BeginInvoke(new Action(() => this.RemoveComponents()));

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

            if (visible)
                this.BeginInvoke(new Action(() => this.AddComponents()));
        }

        public AddAppUI()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.Width = 512;

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
        }
    }
}
