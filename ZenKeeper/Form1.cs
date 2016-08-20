using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZenKeeper
{
    public partial class Form1 : Form
    {
        bool stay = false;
        int sItemT = -1;

        Dictionary<string, string> pCache = new Dictionary<string, string>();

        IntPtr process = IntPtr.Zero;
        IntPtr pThis = IntPtr.Zero;

        KeyboardHook hook = new KeyboardHook();

        private const int SW_SHOWNORMAL = 1;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        public void SetForeground(IntPtr proc)
        {
            if (proc != IntPtr.Zero)
            {
                SetForegroundWindow(proc);
            }
        }

        public void addEdit(string item, string outl)
        {
            if (sItemT > -1)
            {
                pCache.Remove(pList.Items[sItemT].ToString());
                pCache.Add(item, outl);
                pList.Items[sItemT] = item;
                File.WriteAllLines(Environment.GetEnvironmentVariable("zkdir"), pCache.Select(x => x.Key + "`" + x.Value));
            }
        }

        public bool itemExists(string item)
        {
            return pList.Items.Contains(item);
        }

        public void addNew(string item, string outl)
        {
            pList.Items.Add(item);
            pCache.Add(item, outl);
            File.WriteAllLines(Environment.GetEnvironmentVariable("zkdir"), pCache.Select(x => x.Key + "`" + x.Value));
        }

        public Form1()
        {
            InitializeComponent();
            hook.KeyPressed +=
            new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(ZenKeeper.ModifierKeys.Control | ZenKeeper.ModifierKeys.Alt | ZenKeeper.ModifierKeys.Shift, Keys.Q); // The hotkey to show the form.
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            process = GetForegroundWindow();
            int mx = MousePosition.X;
            int my = MousePosition.Y;
            if (MousePosition.Y + this.Height > Screen.FromControl(this).Bounds.Bottom)
                my = MousePosition.Y - this.Height;
            if (MousePosition.X + this.Width > SystemInformation.VirtualScreen.Right)
                mx = MousePosition.X - this.Width;
            this.Location = new Point(mx, my);
            this.Show();
            SetForeground(pThis);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Environment.SetEnvironmentVariable("a6gf03nm", "9348yfggndhcuiasss"); // It is a good idea to change this for your own use. No significance to it being an environment variable.
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\config.txt"))
            {
                MessageBox.Show("Please choose a path to store your encrypted passwords in.");
                DialogResult store = folderBrowserDialog1.ShowDialog();
                if (store == DialogResult.OK)
                {
                    Environment.SetEnvironmentVariable("zkdir", folderBrowserDialog1.SelectedPath + @"\zk.txt");
                    File.WriteAllText(Directory.GetCurrentDirectory() + @"\config.txt", folderBrowserDialog1.SelectedPath);
                }
                else
                {
                    MessageBox.Show("You have to choose somewhere to store your data.");
                    this.Close();
                }
            }
            else
            {
                string tdir = File.ReadAllText(Directory.GetCurrentDirectory() + @"\config.txt").Trim() + @"\zk.txt";
                Environment.SetEnvironmentVariable("zkdir", tdir);
                if (File.Exists(tdir))
                {
                    foreach (string line in File.ReadAllLines(tdir))
                    {
                        string[] p = line.Split('`');
                        pCache.Add(p[0], p[1]);
                        pList.Items.Add(p[0]);
                    }
                    tdir = "";
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            hook.Dispose();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            pThis = GetForegroundWindow();
            this.Hide();
        }

        private void pList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
        }

        private void pList_DoubleClick(object sender, EventArgs e)
        {
            if (pList.SelectedIndex > -1)
            {
                string v = pList.SelectedItem.ToString();
                string get;
                if (pCache.TryGetValue(v, out get))
                {
                    MasterPass mp = new MasterPass();
                    mp.ShowDialog();
                    if (mp.DialogResult == DialogResult.OK)
                    {
                        string s = Environment.GetEnvironmentVariable("a6gf03nm");
                        string d = "";
                        try
                        {
                            d = CryptoURL.DecryptString(get, s.Insert((int)Math.Floor(s.Length/3.5), mp.rvmp));
                        }
                        catch (Exception exr)
                        {
                            MessageBox.Show("Error decrypting. No value set for item?");
                        }
                        s = "";
                        if (d != "ERROR")
                        {
                            SetForeground(process);
                            SendKeys.Send(d); // Some web forms don't seem to like this? Perhaps sending a message event to the process would work better.
                        }
                        else
                            MessageBox.Show("Error decrypting. Wrong Master Password?");
                    }
                }
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            // Lost focus
            if (!stay)
            {
                pList.ClearSelected();
                this.Hide();
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            stay = false;
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            New newf = new New(this);
            stay = true;
            newf.ShowDialog();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pList.SelectedIndex > -1)
            {
                string v = pList.SelectedItem.ToString();
                string get;
                if (pCache.TryGetValue(v, out get))
                {
                    New newf = new New(this, v, get);
                    stay = true;
                    sItemT = pList.SelectedIndex;
                    newf.ShowDialog();
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pList.SelectedIndex > -1)
            {
                stay = true;
                DialogResult dr = MessageBox.Show("Are you sure you want to delete this?", "WARNING!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    if (pList.SelectedIndex > -1)
                    {
                        string v = pList.SelectedItem.ToString();
                        pCache.Remove(v);
                        pList.Items.Remove(v);
                        File.WriteAllLines(Environment.GetEnvironmentVariable("zkdir"), pCache.Select(x => x.Key + "`" + x.Value));
                    }
                }
            }
        }

        /* Unimplemented. */
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            opts Opts = new opts(this);
            stay = true;
            Opts.ShowDialog();
        }
    }

    public sealed class KeyboardHook : IDisposable
    {
        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        private class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            public Window()
            {
                // create the handle for the window.
                this.CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg == WM_HOTKEY)
                {
                    // get the keys.
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    // invoke the event to notify the parent.
                    if (KeyPressed != null)
                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            #region IDisposable Members

            public void Dispose()
            {
                this.DestroyHandle();
            }

            #endregion
        }

        private Window _window = new Window();
        private int _currentId;

        public KeyboardHook()
        {
            // register the event of the inner native window.
            _window.KeyPressed += delegate(object sender, KeyPressedEventArgs args)
            {
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };
        }

        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            // increment the counter.
            _currentId = _currentId + 1;

            // register the hot key.
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Couldn’t register the hot key.");
        }

        /// <summary>
        /// A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        #region IDisposable Members

        public void Dispose()
        {
            // unregister all the registered hot keys.
            for (int i = _currentId; i > 0; i--)
            {
                UnregisterHotKey(_window.Handle, i);
            }

            // dispose the inner native window.
            _window.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs
    {
        private ModifierKeys _modifier;
        private Keys _key;

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            _modifier = modifier;
            _key = key;
        }

        public ModifierKeys Modifier
        {
            get { return _modifier; }
        }

        public Keys Key
        {
            get { return _key; }
        }
    }

    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum ModifierKeys : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
