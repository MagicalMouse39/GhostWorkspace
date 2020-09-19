using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GhostWorkspace
{
    public partial class SelectProcessUI : Form
    {
        public List<Process> SelectedProcesses;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public SelectProcessUI()
        {
            InitializeComponent();

            this.SelectedProcesses = new List<Process>();
            this.DialogResult = DialogResult.Cancel;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = PanelUI.Settings.SettingsBG;
            this.Opacity = PanelUI.Settings.SettingsAlpha;

            Label title = new Label() { Text = "Select a Process", Font = new Font("Bahnschrift SemiBold", 20F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))), Top = 10, Height = 40, Width = 180 };
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
