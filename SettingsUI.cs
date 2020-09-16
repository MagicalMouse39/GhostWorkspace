using GhostWorkspace.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GhostWorkspace
{
    public partial class SettingsUI : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public SettingsUI()
        {
            InitializeComponent();

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
