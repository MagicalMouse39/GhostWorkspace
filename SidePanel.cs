using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GhostWorkspace
{
    class SidePanel : Panel
    {
        private Color color;

        public SidePanel()
        {
            this.color = PanelUI.Settings.SidePanelBG;

            //this.Dock = DockStyle.Left;
            this.BackColor = this.color;
            this.BorderStyle = BorderStyle.None;
            this.Region = Region.FromHrgn(InteropUtils.CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
