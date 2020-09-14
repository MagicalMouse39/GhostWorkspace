using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GhostWorkspace
{
    public partial class UI : Form
    {
        public static Settings Settings;
        private List<string> applications;

        private Rectangle mainScreen = Screen.PrimaryScreen.Bounds;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                return cp;
            }
        }

        public void CheckSavings()
        {
            if (!File.Exists("settigs.json"))
            {
                var sw = new StreamWriter("settings.json");
                var s = JsonSerializer.Create();
                s.Serialize(sw, Settings);
                sw.Close();

                sw = new StreamWriter("applications.json");
                s.Serialize(sw, new string[] { });
                sw.Close();
            }

            var sr = new StreamReader("settings.json");
            var data = sr.ReadToEnd();
            Settings = JsonConvert.DeserializeObject<Settings>(data);
            sr.Close();

            sr = new StreamReader("applications.json");
            data = sr.ReadToEnd();
            this.applications = JsonConvert.DeserializeObject<List<string>>(data);
            sr.Close();
        }

        public void CreateSidePanel()
        {
            SidePanel panel = new SidePanel() { Width = 50, Height = this.applications.Count * 40 + 1 };
            panel.Top = (this.mainScreen.Height - panel.Height) / 2;

            this.Controls.Add(panel);

        }

        public UI()
        {
            InitializeComponent();

            Settings = new Settings();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Width = this.mainScreen.Width;
            this.Height = this.mainScreen.Height;
            this.BackColor = Color.Bisque;
            this.TransparencyKey = Color.Bisque;

            CheckSavings();
            CreateSidePanel();
        }
    }
}
