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
    public partial class Form2 : Form
    {
        //регистрация
        public Form2()
        {
            InitializeComponent();
        }

        //эфект для кнопки "Назад"
        private void button2_MouseEnter_1(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Blue; //надпись становиться голубой при наведении
        }

        private void button2_MouseLeave_1(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Black; //надпись снова становиться чёрной при выходе из фокуса
        }

        //действия при закрытии формы
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit(); //закрывается вся программа
        }

        private void button2_Click(object sender, EventArgs e) //возврат на форму авторизации
        {
            Form1 f1 = new Form1();
            this.Hide();
            f1.Show();
        }

        FileStream file;
        StreamReader reader;
        StreamWriter writer;
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox4.Text == "") //заполнены ли все поля
            {
                MessageBox.Show("Заполните, пожалуйста все поля");
                return;
            }
            if(textBox4.Text.Length < 4) //длинна логина 
            {
                MessageBox.Show("Длина логина должна быть не менее 4 символов");
                return;
            }
            if (textBox1.Text != textBox2.Text) //корректный повтор пароля
            {
                MessageBox.Show("Вы ввели разные пароли");
                return;
            }
            if (textBox2.Text.Length < 6) //длинна пароля
            {
                MessageBox.Show("Длина пароля должна быть не менее 6 символов");
                return;
            }
            //считываем все данные из файла
            file = new FileStream("Accounts.txt", FileMode.OpenOrCreate);
            reader = new StreamReader(file);
            string text = reader.ReadToEnd();
            reader.Close();
            file.Close();
            string [] words  = text.Split(new char[] { '\n' });
            //проверяем занят ли указанный логин
            for (int i = 0; i < words.Length; i+=2)
            {
                if (textBox4.Text == words[i])
                {
                    MessageBox.Show("Данный логин уже занят");
                    return;
                }
            }
            //открываем файл для записи в конец
            file = new FileStream("Accounts.txt", FileMode.Append);
            writer = new StreamWriter(file);
            writer.WriteLine(textBox4.Text); //каждая запись с новой строки
            writer.WriteLine(textBox2.Text);
            writer.Close();
            file.Close();
            MessageBox.Show("Добро пожаловать, " + textBox4.Text); //переход на основную форму
            Form3 f3 = new Form3();
            this.Hide();
            f3.Show();
        }
    }
}
