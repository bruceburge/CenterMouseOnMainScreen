using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CenterMouseOnMainScrean
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalAddAtom(string lpString);
        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalDeleteAtom(short nAtom);
        public short HotkeyID { get; private set; }

        private HotKeyRegister hotKeyRegister = null;
        public Form1()
        {
            // Create a unique Id for this class in this instance
            string atomName = Thread.CurrentThread.ManagedThreadId.ToString("X8") + this.GetType().FullName;
            HotkeyID = GlobalAddAtom(atomName);

            InitializeComponent();
            hotKeyRegister = new HotKeyRegister(this.Handle, HotkeyID, KeyModifiers.Control | KeyModifiers.Alt, Keys.H);

            hotKeyRegister.HotKeyPressed += HotKeyer_HotKeyPressed;

        }

        private void HotKeyer_HotKeyPressed(object sender, EventArgs e)
        {
            CenterMouseOnPrimaryScreen();
        }

        public void CenterMouseOnPrimaryScreen()
        {
            // Main monitor
            Screen s = Screen.PrimaryScreen;

            // Working area after subtracting task bar/widget area etc...
            Rectangle b = s.WorkingArea;

            var centerPoint = new Point(b.Left + (b.Width / 2), b.Top + (b.Height / 2));

            SetCursorPos(centerPoint.X, centerPoint.Y);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.HotkeyID != 0)
            {
                hotKeyRegister.HotKeyPressed += HotKeyer_HotKeyPressed;
                hotKeyRegister.Dispose();
                // clean up the atom list
                GlobalDeleteAtom(HotkeyID);
                HotkeyID = 0;

            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
