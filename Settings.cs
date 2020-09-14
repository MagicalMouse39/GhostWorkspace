﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostWorkspace
{
    class Settings
    {
        public bool GhostShift = false;
        public bool GhostCtrl = false;
        public bool GhostAlt = true;
        public char GhostKey = 'h';

        public Color SidePanelBG = Color.FromArgb(10, Color.OrangeRed);
    }
}