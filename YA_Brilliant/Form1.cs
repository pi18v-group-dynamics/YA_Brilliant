using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        FileStream file;
        StreamReader reader;
        StreamWriter writer;

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "") //заполнены ли все поля
            {
                MessageBox.Show("Заполните, пожалуйста, все поля");
                return;
            }
            //считываем все данные из файла
            file = new FileStream("Accounts.txt", FileMode.OpenOrCreate);
            reader = new StreamReader(file);
            string text = reader.ReadToEnd();
            reader.Close();
            file.Close();
            string[] words = text.Split('\n');
            //проверяем существует ли указанный логин
            for (int i = 0; i < words.Length; i += 2)
            {
                if (words[i] == textBox1.Text)
                {
                    if(words[i+1] == textBox2.Text)
                    {
                        MessageBox.Show("Добро пожаловать, " + textBox1.Text); //переход на основную форму
                        file = new FileStream("Activity.txt", FileMode.Append);
                        writer = new StreamWriter(file);
                        writer.WriteLine("В систему вошёл пользователь: " + textBox1.Text + " " + DateTime.Now); //журналирование
                        writer.Close();
                        file.Close();
                        Form3 f3 = new Form3();
                        this.Hide();
                        f3.Show();
                        return;
                    }
                }
            }
            MessageBox.Show("Возможно вы ввели неверный логин или пароль");
            textBox2.Text = "";
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