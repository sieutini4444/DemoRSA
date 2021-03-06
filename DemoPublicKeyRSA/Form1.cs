using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoPublicKeyRSA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Encode f2 = new Encode();
            f2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Decode f3 = new Decode();
            f3.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Check f4 = new Check();
            f4.Show();
        }
    }
}
