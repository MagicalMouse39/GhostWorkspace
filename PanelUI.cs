using GhostWorkspace.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GhostWorkspace.InteropUtils;

namespace GhostWorkspace
{
    public partial class PanelUI : Form
    {
        public static Settings Settings;
        private List<string> applications;
        private SettingsUI settingsUI;

        private bool animating = false;
        private bool anIn = false;
        private enum Animation { In, Out }

        private Rectangle mainScreen = Screen.PrimaryScreen.Bounds;

        /*
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                return cp;
            }
        }
        */

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

        private void AnimatePanel(Animation an)
        {
            switch (an)
            {
                case Animation.In:
                    Task.Factory.StartNew(() =>
                    {
                        if (this.anIn)
                            return;
                        this.animating = false;
                        this.animating = true;
                        this.anIn = true;
                        this.BeginInvoke(new Action(() => this.Visible = true));
                        for (int i = (-1 * this.Width) - 10; i < 20; i++)
                        {
                            if (!this.animating || !this.anIn)
                                break;
                            Thread.Sleep(2);
                            this.BeginInvoke(new Action(() => this.Left = i));
                        }
                        this.animating = false;
                        this.anIn = false;
                    });
                    break;

                case Animation.Out:
                    Task.Factory.StartNew(() =>
                    {
                        this.anIn = false;
                        this.animating = false;
                        this.animating = true;
                        for (int i = this.Left; i > (-1 * this.Width) - 10; i--)
                        {
                            if (!this.animating)
                                break;
                            Thread.Sleep(2);
                            this.BeginInvoke(new Action(() => this.Left = i));
                        }
                        this.BeginInvoke(new Action(() => this.Visible = false));
                        this.animating = false;
                    });
                    break;
            }
        }

        public PanelUI()
        {
            InitializeComponent();

            Settings = new Settings();

            this.settingsUI = new SettingsUI();

            CheckSavings();

            this.FormBorderStyle = FormBorderStyle.None;
            
            this.Width = 100;
            this.Height = (this.applications.Count + 1) * 100;
            this.Top = (this.mainScreen.Height - this.Height) / 2;
            this.Left = 20;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.BackColor = Settings.SidePanelBG;
            this.Opacity = Settings.SidePanelAlpha;

            #region SettingsButton
            Button settingsButton = new Button() { Width = 80, Height = 80, ForeColor = Color.White, Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, 80, 80, 20, 20)), Left = 10, Top = 10, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 50, 151, 168), BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Resources.SettingsIcon };
            settingsButton.Click += (s, e) =>
            {
                this.AnimatePanel(Animation.In);
                settingsUI.ShowDialog();
            };
            this.Controls.Add(settingsButton);
            #endregion

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));
            
            //Mouse detection for panel show
            Task.Factory.StartNew(() =>
            {
                while (!this.IsDisposed)
                {
                    POINT mousePos = new POINT();
                    InteropUtils.GetCursorPos(out mousePos);
                    int mouseX = mousePos.X;

                    if (mouseX > 0 && mouseX < 150 && this.Left <= (-1 * this.Width) - 10 && !this.anIn)
                        AnimatePanel(Animation.In);
                    else if ((mouseX > 150 || mouseX < 0) && this.Left > 0 && !this.settingsUI.Visible)
                        AnimatePanel(Animation.Out);
                }
            });

            //AddApplications();
        }
    }
}
