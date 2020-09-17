using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GhostWorkspace
{
    public static class InteropUtils
    {
        #region Types
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point) =>
                new Point(point.X, point.Y);
        }
        #endregion

        /// <summary>
        /// Returns a custom region, given a rectangle and the two corner radiuses
        /// </summary>
        /// <param name="nLeftRect">X coord for upper-left corner</param>
        /// <param name="nTopRect">Y coord for upper-left corner</param>
        /// <param name="nRightRect">X coord for lower-right corner</param>
        /// <param name="nBottomRect">Y coord for lower-right corner</param>
        /// <param name="nWidthEllipse">Height of ellipse</param>
        /// <param name="nHeightEllipse">Width of ellipse</param>
        /// <returns></returns>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        /// <summary>
        /// Returns the mouse position
        /// </summary>
        /// <param name="lpPoint">OUT param for output</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern int ShowWindow(int hwnd, int nCmdShow);

        [DllImport("user32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
