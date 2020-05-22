using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YA_Brilliant
{
    public partial class Form4 : Form
    {
        static public Bitmap begin, end;

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = begin;
            pictureBox2.Image = end;
        }

        public Form4()
        {
            InitializeComponent();
        }
    }
}
