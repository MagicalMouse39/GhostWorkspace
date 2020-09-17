using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostWorkspace
{
    public class Settings
    {
        public bool GhostShift = false;
        public bool GhostCtrl = false;
        public bool GhostAlt = true;
        public char GhostKey = 'H';

        public bool CloseProcessesAtExit = false;

        public Color SidePanelBG = Color.OrangeRed;
        public float SidePanelAlpha = .90f;

        public Color SettingsBG = Color.OrangeRed;
        public float SettingsAlpha = .80f;
    }
}
