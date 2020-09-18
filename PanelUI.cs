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
        public static List<string> Applications;

        private SettingsUI settingsUI;
        private AddAppUI addAppUI;
        private ProcessManager processManager;

        private bool animating = false;
        private bool anIn = false;
        private enum Animation { In, Out }

        private Rectangle mainScreen = Screen.PrimaryScreen.Bounds;

        public static PanelUI instance;

        public static void SaveChanges()
        {
            var sw = new StreamWriter("settings.json");
            var s = JsonSerializer.Create();
            s.Serialize(sw, Settings);
            sw.Close();

            sw = new StreamWriter("applications.json");
            s.Serialize(sw, Applications.ToArray<string>());
            sw.Close();
        }

        public void CheckSavings()
        {
            if (!File.Exists("settings.json") || !File.Exists("applications.json"))
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
            Applications = JsonConvert.DeserializeObject<List<string>>(data);
            sr.Close();
        }

        public static void ChangeGhostKey() =>
                PanelUI.ChangeGhostKey((PanelUI.Settings.GhostAlt ? 1 : 0) + (PanelUI.Settings.GhostCtrl ? 2 : 0) + (PanelUI.Settings.GhostShift ? 4 : 0), PanelUI.Settings.GhostKey);

        public static void ChangeGhostKey(int Special, int Key)
        {
            UnregisterHotKey(PanelUI.instance.Handle, 1);
            RegisterHotKey(PanelUI.instance.Handle, 1, Special, Key);
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

        public void RemoveApplication(string app)
        {
            Applications.Remove(app);
            
            foreach (Control c in this.Controls)
                if (c.Name == app)
                    this.Controls.Remove(c);

            this.Width = 100;
            this.Height -= 95;
            this.Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));

            SaveChanges();
        }

        public void AddApplication(string app, bool addToList)
        {
            if (addToList)
                Applications.Add(app);

            this.Width = 100;
            this.Height += 95;
            this.Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));

            int top = (Applications.Count + 2) * 95;
            var icon = Icon.ExtractAssociatedIcon(app).ToBitmap();
            Button btn = new Button() { Name = app, Width = 80, Height = 80, ForeColor = Color.White, Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, 80, 80, 20, 20)), Left = 10, Top = top, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 50, 151, 168), BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = icon };
            btn.Click += (s, e) => ProcessManager.instance.HandleApp(app);
            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Remove");
            cm.MenuItems[0].Click += (s, e) => RemoveApplication(app);
            btn.ContextMenu = cm;
            this.Controls.Add(btn);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1)
            {
                this.Visible = this.processManager.ProcessesHidden;
                
                if (this.settingsUI.Visible)
                    this.settingsUI.Visible = false;
                if (this.addAppUI.Visible)
                    this.addAppUI.Visible = false;

                if (this.processManager.ProcessesHidden)
                    this.processManager.ShowProcesses();
                else
                    this.processManager.HideProcesses();
            }
            base.WndProc(ref m);
        }

        public void UpdateColors()
        {
            this.BackColor = Settings.SidePanelBG;
            this.Opacity = Settings.SidePanelAlpha;
        }

        public PanelUI()
        {
            instance = this;

            InitializeComponent();
            this.ShowInTaskbar = false;

            Settings = new Settings();
            Applications = new List<string>();

            InteropUtils.RegisterHotKey(this.Handle, 1, (Settings.GhostAlt ? 1 : 0) + (Settings.GhostCtrl ? 2 : 0) + (Settings.GhostShift ? 4 : 0), Settings.GhostKey);

            this.settingsUI = new SettingsUI();
            this.addAppUI = new AddAppUI();
            this.processManager = new ProcessManager();

            CheckSavings();

            this.FormBorderStyle = FormBorderStyle.None;
            
            this.Width = 100;
            this.Height = 95 * 3;
            this.Top = (this.mainScreen.Height - this.Height) / 2;
            this.Left = 20;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.BackColor = Settings.SidePanelBG;
            this.Opacity = Settings.SidePanelAlpha;

            #region SettingButtons
            Button exitButton = new Button() { Width = 80, Height = 80, ForeColor = Color.White, Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, 80, 80, 20, 20)), Left = 10, Top = 10, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 50, 151, 168), BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Resources.ExitIcon };
            exitButton.Click += (s, e) => this.Close();
            Button settingsButton = new Button() { Width = 80, Height = 80, ForeColor = Color.White, Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, 80, 80, 20, 20)), Left = 10, Top = 100, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 50, 151, 168), BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Resources.SettingsIcon };
            settingsButton.Click += (s, e) =>
            {
                if (this.Left < 0)
                    this.AnimatePanel(Animation.In);
                
                settingsUI.ShowDialog();
            };
            Button addAppButton = new Button() { Width = 80, Height = 80, ForeColor = Color.White, Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, 80, 80, 20, 20)), Left = 10, Top = 190, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 50, 151, 168), BackgroundImageLayout = ImageLayout.Stretch, BackgroundImage = Resources.PlusIcon };
            addAppButton.Click += (s, e) => addAppUI.ShowDialog();
            
            this.Controls.Add(exitButton);
            this.Controls.Add(settingsButton);
            this.Controls.Add(addAppButton);
            #endregion

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));

            this.FormClosing += (s, e) =>
            {
                if (Settings.CloseProcessesAtExit)
                    this.processManager.ClearProcesses();
            };

            //Mouse detection for panel show
            Task.Factory.StartNew(() =>
            {
                while (!this.IsDisposed)
                {
                    if (this.processManager.ProcessesHidden)
                        continue;

                    POINT mousePos = new POINT();
                    InteropUtils.GetCursorPos(out mousePos);
                    int mouseX = mousePos.X;

                    if (mouseX > 0 && mouseX < 150 && this.Left <= (-1 * this.Width) - 10 && !this.anIn)
                        AnimatePanel(Animation.In);
                    else if ((mouseX > 150 || mouseX < 0) && this.Left > 0 && !this.settingsUI.Visible)
                        AnimatePanel(Animation.Out);
                }
            });

            foreach (var app in Applications)
                this.AddApplication(app, false);
        }
    }
}
