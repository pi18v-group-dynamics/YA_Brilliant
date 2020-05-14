using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace YA_Brilliant
{
    public partial class Form3 : Form
    {
        //основная форма
        public Form3()
        {
            InitializeComponent();
        }

        DialogResult dr; //окошко да/нет

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) //если в пикчер боксе есть изображение то спрашиваем: нужно ли сохранить
            {
                dr = MessageBox.Show("Сохранить старое изображение перед добавлением нового?", "Внимание!", MessageBoxButtons.YesNo);
                if(dr == DialogResult.Yes) //если да, то сохраняем и добавляем новое
                {
                    ImSave();
                    addImage();
                }
                else // если нет, то просто добавляем новое
                {
                    addImage();
                }
            }
            else
            {
                addImage();
            }
        }

        bool save = false;
        private void ImSave()
        {
            if (save)
            {
                MessageBox.Show("Вы только что сохранили изображение");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "(*.png; *.bmp; *.jpg; *.jpeg)|*.png; *.bmp; *.jpg; *.jpeg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(sfd.FileName);
                save = true;
            }
            else return;
        }

        private void addImage() //функция добавления изображения
        {
            dr = MessageBox.Show("Вы хотите ввести путь вручную?", "Добавление изображения", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes) //если вручную, то выводим окошко с текстовым полем
            {
                string path = Interaction.InputBox("Введите путь к изображению", "Добавление изображения", ""); //получаем адрес
                if (path.Length == 0) return;
                else
                {
                    AddToPicBox(path);
                }
            }
            else
            {// иначе через диалоговое
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "(*.png; *.bmp; *.jpg; *.jpeg)|*.png; *.bmp; *.jpg; *.jpeg";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    AddToPicBox(ofd.FileName);
                }
            }
        }

        List<Bitmap> bmpList = new List<Bitmap>();

        private void AddToPicBox(string path) //зная адрес, добавляем изображение в пикчер бокс
        {
            comboBox1.Text = "Эффекты";
            comboBox2.Text = "Фильтры";
            pictureBox1.InitialImage = null;
            bmpList.Clear();
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var ms = new MemoryStream((int)fs.Length);
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    pictureBox1.Image = Image.FromStream(ms);
                    Bitmap bmp = new Bitmap(pictureBox1.Image);
                    bmpList.Add(bmp);
                    save = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e) //реализация эффектов
        {
            if (bmpList.Count == 0)
            {
                MessageBox.Show("Выполните добавление изображения");
                return;
            }
            save = false;
            
            switch(comboBox1.Text)
            {
                case "Линейная коррекция":
                    Bitmap bmp1, bmp2;
                    bmp1 = bmpList.Last();
                    bmp2 = bmp1;
                    linCor(bmp1, bmp2);
                    break;
                case "Нелинейная коррекция":
                    pictureBox1.Image = Gamma(pictureBox1.Image, 2);
                    break;
                case "Зашумление":
                    break;
                case "Стекло":
                    break;
                case "Волна":
                    break;
            }
        }

        int XmaxR, XminR, XmaxG, XminG, XmaxB, XminB;

        private void linCor(Bitmap bmp1, Bitmap bmp2) //линейная коррекция
        {
            Xmaxmin(bmp1);
            for (int i = 0; i < bmp1.Width; i++)
            {
                for (int j = 0; j < bmp1.Height; j++)
                {
                    Color pixColor = bmp1.GetPixel(i, j);
                    bmp2.SetPixel(i, j, System.Drawing.Color.FromArgb(Y(Convert.ToInt32(pixColor.R), XmaxR, XminR), 
                        Y(Convert.ToInt32(pixColor.G), XmaxG, XminG), Y(Convert.ToInt32(pixColor.B), XmaxB, XminB)));
                }
            }
            pictureBox1.Image = bmp2;
        }
        
        private int Y(int x,int Xmax, int Xmin) //вычисление "нового" цета
        {
            int y = 0; 
            if (Xmax == Xmin) y = x - Xmin;
            else y = (x-Xmin) * (255 / (Xmax - Xmin));
            return y;
        }
        
        private void Xmaxmin(Bitmap bmp)//нахождение максимального и минимального значения каждого цвета
        {           
            XminR = bmp.GetPixel(0, 0).R;
            XmaxR = bmp.GetPixel(0, 0).R;
            XmaxG = bmp.GetPixel(0, 0).G;
            XminG = bmp.GetPixel(0, 0).G;
            XmaxB = bmp.GetPixel(0, 0).B;
            XminB = bmp.GetPixel(0, 0).B;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color pixColor=bmp.GetPixel(i, j);
                    
                    if (XminR > pixColor.R) XminR = pixColor.R;
                    if (XmaxR < pixColor.R) XmaxR = pixColor.R;
                    if (XminG > pixColor.G) XminG = pixColor.G;
                    if (XmaxG < pixColor.G) XmaxG = pixColor.G;
                    if (XminB > pixColor.B) XminB = pixColor.B;
                    if (XmaxB < pixColor.B) XmaxB = pixColor.B;
 
                }
            }
        }
        private Bitmap Gamma(Image image, float gamma)
        {
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetGamma(gamma);
            Point[] points =
            {
                new Point(0, 0),
                new Point(image.Width, 0),
                new Point(0, image.Height),
            };
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            // Создаем изображение результата.
            Bitmap bm = new Bitmap(image.Width, image.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
            }
            bmpList.Add(bm);
            // Вернуть результат.
            return bm;
        }

       /* private Bitmap Gamma(Image image, float gamma)
        {
            // Устанавливаем гамма-значение объекта ImageAttributes.
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetGamma(gamma);
            // Нарисуем изображение на новом растровом изображении
            // при применении нового значения гаммы.
            Point[] points =
            {
                new Point(0, 0),
                new Point(image.Width, 0),
                new Point(0, image.Height),
            };
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            // Создаем растровое изображение результата.
            Bitmap bm = new Bitmap(image.Width, image.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
            }

            // Вернуть результат.
            return bm;
        }*/

        private void button3_Click(object sender, EventArgs e) //save
        {
            if (pictureBox1.Image != null) ImSave();
            else MessageBox.Show("Выберите изображение");
        }



        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                dr = MessageBox.Show("Сохранить старое изображение перед добавлением нового?", "Внимание!", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes) //если да, то сохраняем и добавляем новое
                {
                    ImSave();
                    bmpList.Clear();
                }
            }
            Application.Exit();
        }
    }
}