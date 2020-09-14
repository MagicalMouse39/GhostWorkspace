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
            this.color = UI.Settings.SidePanelBG;

            this.Dock = DockStyle.Left;
            this.BackColor = this.color;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawEllipse(new Pen(this.color), new Rectangle(this.Left, this.Top, this.Width, this.Height));
            base.OnPaint(e);
        }
    }
}
