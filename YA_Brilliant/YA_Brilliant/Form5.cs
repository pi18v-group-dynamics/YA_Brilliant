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
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        static public int typeWindow = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            if (typeWindow == 0)
            {
                try
                {
                    double val = Convert.ToDouble(textBox1.Text);
                    if (val <= 0)
                    {
                        MessageBox.Show("Введите коэффициент строго больше нуля");
                        return;
                    }
                    Form3.gamma = (float)(val);
                    Form3.SetValue = true;
                    this.Hide();
                }
                catch
                {
                    MessageBox.Show("Введите данные корректно ( > 0)");
                }
            }
            else
            {
                try
                {
                    int ugol = Convert.ToInt32(textBox2.Text);
                    if (ugol <=0 || ugol >180)
                    {
                        MessageBox.Show("Введите угол от 1 до 180!");
                        return;
                    }
                    Form3.ugol = ugol;
                    Form3.SetValue = true;
                    this.Hide();
                }
                catch
                {
                    MessageBox.Show("Введите данные корректно");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3.SetValue = false;
            this.Hide();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8 && ch != ',')
                e.Handled = true;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            if (typeWindow == 0)
                panel1.Visible = true;
            else panel2.Visible = true;
        }
    }
}