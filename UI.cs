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
        private Settings settings;
        private string[] applications;

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
                s.Serialize(sw, this.settings);
                sw.Close();

                sw = new StreamWriter("applications.json");
                s.Serialize(sw, new string[] { });
                sw.Close();
            }

            this.settings = 
        }

        public UI()
        {
            Rectangle sc = Screen.PrimaryScreen.Bounds;

            InitializeComponent();

            this.settings = new Settings();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Width = sc.Width;
            this.Height = sc.Height;
            this.BackColor = Color.Bisque;
            this.TransparencyKey = Color.Bisque;

            CheckSavings();
            //CreateSidePanel();
        }
    }
}
