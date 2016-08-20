using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZenKeeper
{
    public partial class New : Form
    {
        Dictionary<byte, string> pchars = new Dictionary<byte,string>(); 
        Form1 pr = null;
        bool editing = false;
        string orig = "";
        string oenc = "";

        private string dealMP()
        {
            MasterPass mp = new MasterPass();
            mp.ShowDialog();
            if (mp.DialogResult == DialogResult.OK)
            {
                string s = Environment.GetEnvironmentVariable("a6gf03nm");
                return CryptoURL.EncryptString(textBox2.Text, s.Insert(7, mp.rvmp));
            }
            else return "";
        }

        private void failDG(string t)
        {
            DialogResult dr = MessageBox.Show(t, "Uh-oh!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (dr == DialogResult.Cancel)
            {
                this.Close();
            }
        }

        public New(Form1 parent, string item = "", string encpass = "")
        {
            InitializeComponent();
            pr = parent;
            pchars.Add(0, "abcdefghijklmnopqrstuvwxyz");
            pchars.Add(1, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            pchars.Add(2, "-_<>,./?;:\"!@#$&*\\|");
            pchars.Add(3, "1234567890");
            editing = (item != "");
            textBox1.Text = item;
            orig = item;
            oenc = encpass;
        }

        private void New_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Contains("`") && !textBox2.Text.Contains("`"))
            {
                    if (!editing)
                    {
                        if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0)
                        {
                            if (!pr.itemExists(textBox1.Text))
                            {
                                string outline = "";
                                outline = dealMP();
                                pr.addNew(textBox1.Text, outline);
                                this.Close();
                            }
                            else
                            {
                                failDG("Key already exists in collection.");
                            }
                        }
                        else
                            failDG("Something is blank!");
                    }
                    else
                    {
                        if (textBox1.Text != "")
                        {
                            string outline = "";
                            if (textBox2.Text.Length > 0)
                            {
                                outline = dealMP();
                            }
                            else
                                outline = oenc;
                            pr.addEdit(textBox1.Text, outline);
                            this.Close();
                        }
                        else
                            failDG("Something is blank!");
                    }
            }
            else
            {
                DialogResult dr = MessageBox.Show("Invalid characters used. Do not use \"`\".", "Uh-oh!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (dr == DialogResult.Cancel)
                {
                    this.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            Random r = new Random();
            string chars = "";
            foreach(KeyValuePair<byte,string> s in pchars)
                chars += s.Value;
            for (int i = 0; i < r.Next(10, 32); i++)
            {
                textBox2.Text += chars[r.Next(0,chars.Length)];
            }
        }

        private void spacesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (spacesToolStripMenuItem.Checked)
                pchars.Add(4, " ");
            else
                pchars.Remove(4);
        }

        private void symbolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (symbolsToolStripMenuItem.Checked)
                pchars.Add(2, "-_<>,./?;:\"!@#$&*\\|");
            else
                pchars.Remove(2);
        }

        private void uppercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (uppercaseToolStripMenuItem.Checked)
                pchars.Add(1, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            else
                pchars.Remove(1);
        }

        private void lowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lowercaseToolStripMenuItem.Checked)
                pchars.Add(0, "abcdefghijklmnopqrstuvwxyz");
            else
                pchars.Remove(0);
        }

        private void showPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox2.PasswordChar = showPasswordToolStripMenuItem.Checked ? '\0' : '*';
        }

        private void numbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (numbersToolStripMenuItem.Checked)
                pchars.Add(3, "1234567890");
            else
                pchars.Remove(3);
        }
    }
}
