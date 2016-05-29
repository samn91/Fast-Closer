using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace Nice_Closer
{
    public partial class Form1 : Form
    {
        [DllImport("user32")]
        public static extern bool GetCursorPos(out Point p);
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point p);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);//lpdwprocessId = IntPtr.Zero and it's return the processId  

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        Process process;
        const int ALT = 0x0001;
        const int CTRL = 0x0002;

        public Form1()
        {
            InitializeComponent();
           
        }

        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Cross;
            AutoSize = false;
            Size = new Size();
        }

        private void label4_MouseUp(object sender, MouseEventArgs e)
        {

            try
            {
                Cursor = Cursors.Default;
                AutoSize = true;
                process = getprocess();
                showinfo();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Restart();
            }

        }

        private void showinfo()
        {
            label5.Text = process.ProcessName;
            label6.Text = process.MainModule.FileName;
            if (process.Responding)
                label7.Text = "Responding";
            else
                label7.Text = "Not Responding";
        }

        Process getprocess()
        {
            killToolStripMenuItem.Enabled = true;
            restartToolStripMenuItem.Enabled = true;
            Point position;
            GetCursorPos(out position);
            uint fe = 0;
            GetWindowThreadProcessId(WindowFromPoint(position), out fe);
            return Process.GetProcessById((int)fe);
        }

        protected override void WndProc(ref Message m)//function that detect the hot key
        {
            try
            {
                if (m.Msg == 0x0312)
                {

                    if (Visible)
                        fHide();
                    else
                        fShow();

                }

                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void fShow()
        {
            Show();
            process = getprocess();
            showinfo();
        }

        private void fHide()
        {
            Hide();
            process = null;
            killToolStripMenuItem.Enabled = false;
            restartToolStripMenuItem.Enabled = false;
            label5.Text = "";
            label6.Text = "";
            label7.Text = "";

        }
       

        private void killToolStripMenuItem_Click(object sender, EventArgs e)
        {
            process.Kill();
            killToolStripMenuItem.Enabled = false;
            restartToolStripMenuItem.Enabled = false;
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fHide();
            notifyIcon1.ShowBalloonTip(1000, "CTRL+ALT+S", "Use The Hotkey To Show/Hide The Application", ToolTipIcon.Info);
        }

      
        private void Form1_Load(object sender, EventArgs e)
        {
            killToolStripMenuItem.Enabled = false;
            restartToolStripMenuItem.Enabled = false;
            notifyIcon1.Text = "Click Icon To Show The Application";
            notifyIcon1.Icon = Icon;
            notifyIcon1.ShowBalloonTip(1000, "CTRL+ALT+S", "Use The Hotkey To Show/Hide The Application", ToolTipIcon.Info);
            RegisterHotKey(Handle, (ALT + CTRL) ^ (int)Keys.S ^ Handle.ToInt32(), (ALT + CTRL), (int)Keys.S);
        }

        private void removeHotkeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnregisterHotKey(Handle, (ALT + CTRL) ^ (int)Keys.S ^ Handle.ToInt32());
        }

        private void showHotkeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ctrl+Alt+S");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(Handle, (ALT + CTRL) ^ (int)Keys.S ^ Handle.ToInt32());
            notifyIcon1.Visible = false;
        }


        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            process.Kill();
            process = Process.Start(process.MainModule.FileName);
        }


    }
}
