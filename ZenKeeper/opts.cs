using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZenKeeper
{
    public partial class opts : Form
    {
        Form1 p = null;
        Dictionary<string, Keys> keys = new Dictionary<string,Keys>();
        public opts(Form1 par)
        {
            InitializeComponent();
            p = par;
        }

        private void opts_Load(object sender, EventArgs e)
        {
            foreach (Keys k in Enum.GetValues(typeof(Keys)))
            {
                keys.Add(k.ToString(), k);
                comboBox1.Items.Add(k.ToString());
            }
        }
    }
}
