using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GhostWorkspace
{
    public partial class ProcessManager
    {
        private Rectangle screen;

        public static ProcessManager instance;

        public static HashSet<Process> processes;

        public bool ProcessesHidden = false;

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        public void HandleApp(string path)
        {
            var testProc = from x in processes where x.ProcessName.Trim() == path.Split('\\').Last().Split('.').First().Trim() select x;

            foreach (var p in testProc)
                InteropUtils.SetForegroundWindow(p.MainWindowHandle);

            if (testProc.Count() <= 0)
                processes.Add(Process.Start(path));
        }

        public void UpdateProcs()
        {
            HashSet<Process> oldProcs = new HashSet<Process>(processes);

            foreach (var p in oldProcs)
            {
                if (p.HasExited)
                    processes.Remove(p);

                foreach (var pr in Process.GetProcessesByName(p.ProcessName))
                    if (!processes.Contains(pr))
                        processes.Add(pr);
            }
        }

        public void HideProcesses()
        {
            this.ProcessesHidden = true;

            this.UpdateProcs();

            foreach (var p in processes)
                if (!p.HasExited)
                    InteropUtils.ShowWindow(p.MainWindowHandle.ToInt32(), SW_HIDE);
        }

        public void ShowProcesses()
        {
            this.ProcessesHidden = false;

            foreach (var p in processes)
                if (!p.HasExited)
                    InteropUtils.ShowWindow(p.MainWindowHandle.ToInt32(), SW_SHOW);
        }

        public void ClearProcesses()
        {
            this.UpdateProcs();

            try
            {
                foreach (var p in processes)
                    p.Kill();
            }
            catch { }
        }

        public ProcessManager()
        {
            instance = this;

            processes = new HashSet<Process>();
        }
    }
}