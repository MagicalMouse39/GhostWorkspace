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

        private ListView lv;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        private bool HasWindowStyle(Process p)
        {
            IntPtr hnd = p.MainWindowHandle;
            UInt32 WS_DISABLED = 0x8000000;
            int GWL_STYLE = -16;
            bool visible = false;
            if (hnd != IntPtr.Zero)
            {
                long style = GetWindowLong(hnd, GWL_STYLE);
                visible = ((style & WS_DISABLED) != WS_DISABLED);
            }
            return visible;
        }

        private void RecalculateProcesses(string searchData)
        {
            this.lv.Items.Clear();
            Task.Factory.StartNew(() =>
            {
                var imgL = new ImageList();

                lv.LargeImageList = imgL;
                lv.SmallImageList = imgL;
                lv.StateImageList = imgL;
                foreach (var proc in from x in Process.GetProcesses() where this.HasWindowStyle(x) select x)
                {
                    if (!proc.ProcessName.ToLower().Contains(searchData.ToLower()))
                        continue;

                    imgL.Images.Add(proc.MainModule.FileName, Icon.ExtractAssociatedIcon(proc.MainModule.FileName));
                    var lvi = lv.Items.Add(proc.ProcessName);
                    lvi.ImageKey = proc.MainModule.FileName;
                }
            });
        }

        public SelectProcessUI()
        {
            InitializeComponent();

            this.SelectedProcesses = new List<Process>();
            this.DialogResult = DialogResult.Cancel;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = PanelUI.Settings.SettingsBG;
            this.Opacity = PanelUI.Settings.SettingsAlpha;

            Label title = new Label() { Text = "Select a Process", Font = new Font("Bahnschrift SemiBold", 20F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))), Top = 10, Height = 40, Width = 250 };
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

            this.lv = new ListView() { Size = new Size(this.Width - 20, this.Height - 100), Top = 75, Left = 10, View = View.List };
            var imgL = new ImageList();

            lv.LargeImageList = imgL;
            lv.SmallImageList = imgL;
            lv.StateImageList = imgL;
            foreach (var proc in from x in Process.GetProcesses() where this.HasWindowStyle(x) select x)
            {
                try
                {
                    var lvi = lv.Items.Add(proc.ProcessName);
                    imgL.Images.Add(proc.MainModule.FileName, Icon.ExtractAssociatedIcon(proc.MainModule.FileName));
                    lvi.ImageKey = proc.MainModule.FileName;
                }
                catch { lv.Items.Add(proc.ProcessName); }
            }
            this.Controls.Add(lv);

            TextBox searchBox = new TextBox() { Width = this.Width - 20, Top = 50, Left = 10 };
            searchBox.TextChanged += (s, e) => this.RecalculateProcesses(searchBox.Text);
            this.Controls.Add(searchBox);

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
