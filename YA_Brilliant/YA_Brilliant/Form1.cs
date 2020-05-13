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
    public partial class Form1 : Form
    {
        //авторизация
        public Form1()
        {
            InitializeComponent();
        }

        //Эффект для кнопки "Регистрация"
        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Blue;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Black;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e) //переход на форму регистрации
        {
            Form2 f2 = new Form2();
            this.Hide();
            f2.Show();
        }
    }
}