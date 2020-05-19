using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        private void button1_Click(object sender, EventArgs e)//добавление изображение
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

        private void ImSave()//функция для сохранения изображения
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "(*.png; *.bmp; *.jpg; *.jpeg)|*.png; *.bmp; *.jpg; *.jpeg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(sfd.FileName);
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

        List<Bitmap> bmpList = new List<Bitmap>(); //список нужен для пошаговой отмены

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
            switch(comboBox1.Text)
            {
                case "Линейная коррекция":
                    pictureBox1.Image = linCor(pictureBox1.Image);
                    break;
                case "Нелинейная коррекция":
                    pictureBox1.Image = Gamma(pictureBox1.Image);
                    break;
                case "Зашумление":
                    pictureBox1.Image = Shum(pictureBox1.Image);
                    break;
                case "Стекло":
                    pictureBox1.Image = Glass(pictureBox1.Image);
                    break;
                case "Волна":
                    pictureBox1.Image = Wave(pictureBox1.Image);
                    break;
                case "Негатив":
                    pictureBox1.Image = Negativ(pictureBox1.Image);
                    break;
            }
        }

        private Bitmap linCor(Image image) //линейная коррекция (по определению не на всех фотографиях эффект не виден)
        {
            Bitmap result = new Bitmap(image);
            Rectangle rect = new Rectangle(0, 0, result.Width, result.Height);
            BitmapData bitmapData = result.LockBits(rect, ImageLockMode.ReadWrite, result.PixelFormat);
            IntPtr ptr = bitmapData.Scan0;
            int bytes = bitmapData.Stride * result.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            byte XmaxR = rgbValues[0], XminR = rgbValues[0],
                XmaxG = rgbValues[1], XminG = rgbValues[1],
                XmaxB = rgbValues[2], XminB = rgbValues[2];
            for (int i = 0; i < rgbValues.Length; i += 4)
            {
                if (XminR > rgbValues[i]) XminR = rgbValues[i];
                if (XmaxR < rgbValues[i]) XmaxR = rgbValues[i];
                if (XminG > rgbValues[i + 1]) XminG = rgbValues[i + 1];
                if (XmaxG < rgbValues[i + 1]) XmaxG = rgbValues[i + 1];
                if (XminB > rgbValues[i + 2]) XminB = rgbValues[i + 2];
                if (XmaxB < rgbValues[i + 2]) XmaxB = rgbValues[i + 2];
            }
            for (int i = 0; i < rgbValues.Length; i += 4)
            {
                rgbValues[i] = Y(rgbValues[i], XmaxR, XminR);
                rgbValues[i + 1] = Y(rgbValues[i + 1], XmaxG, XminG);
                rgbValues[i + 2] = Y(rgbValues[i + 2], XmaxB, XminB);
            }
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            result.UnlockBits(bitmapData);
            bmpList.Add(result);
            return result;
        }

        private byte Y (byte x, byte Xmax, byte Xmin) //вычисление "нового" цета
        {
            byte y;
            if (Xmax == Xmin)  y = (byte)(255);
            else y = (byte)((x-Xmin) * (255 / (Xmax - Xmin)));
            return y;
        }

        static public float gamma;
        static public bool SetValue;

        private Bitmap Gamma(Image image) //нелинейная гамма коррекция
        {
            Bitmap result;
            Form5 f5 = new Form5();
            Form5.typeWindow = 0;
            f5.ShowDialog();
            if (SetValue)
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
                result = new Bitmap(image.Width, image.Height);
                using (Graphics gr = Graphics.FromImage(result))
                {
                    gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
                }
                bmpList.Add(result);
                // Вернуть результат.
                return result;
            }
            result = new Bitmap(image);
            return result;
        }

        private Bitmap Shum(Image image) //зашумление
        {
            Bitmap result = new Bitmap(image);
            Rectangle rect = new Rectangle(0, 0, result.Width, result.Height);
            BitmapData bitmapData = result.LockBits(rect, ImageLockMode.ReadWrite, result.PixelFormat);
            IntPtr ptr = bitmapData.Scan0;
            int bytes = bitmapData.Stride * result.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            Random rand = new Random();
            for (int i = 0; i < rgbValues.Length; i += 4)
            {
                rgbValues[i] = (byte)(rand.Next(0, 2) == 1 ? rgbValues[i] : 255);
                rgbValues[i + 1] = (byte)(rand.Next(0, 2) == 1 ? rgbValues[i + 1] : 255);
                rgbValues[i + 2] = (byte)(rand.Next(0, 2) == 1 ? rgbValues[i + 2] : 255);
            }
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            result.UnlockBits(bitmapData);
            bmpList.Add(result);
            return result;
        }

        private Bitmap Glass(Image image) //стекло
        {
            Random rand = new Random();
            Bitmap bitmap = new Bitmap(image);
            Bitmap result = new Bitmap(image);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    int x = Convert.ToInt32(i + (rand.Next(1, 5) - 0.5) * 10);
                    int y = Convert.ToInt32(j + (rand.Next(1, 5) - 0.5) * 10);
                    Color color = bitmap.GetPixel(i, j);
                    if (x >= bitmap.Width) x = bitmap.Width-1;
                    if (x < 0) x = 0;
                    if (y >= bitmap.Height) y = bitmap.Height-1;
                    if (y < 0) y = 0;
                    result.SetPixel(x, y, color);
                }
            }
            bmpList.Add(result);
            return result;
        }

        static public int ugol = 1;
        private Bitmap Wave(Image image)//волна
        {
            Bitmap bitmap = new Bitmap(image);
            Bitmap result = new Bitmap(image);
            Form5 f5 = new Form5();
            Form5.typeWindow = 1;
            f5.ShowDialog();
            if (SetValue)
            {
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        int x = Convert.ToInt32(i + 20 * Math.Sin((2 * Math.PI * j) / ugol));
                        int y = j;
                        Color color = bitmap.GetPixel(i, j);
                        if (x >= bitmap.Width) x = bitmap.Width - 1;
                        if (x < 0) x = 0;
                        result.SetPixel(x, y, color);
                    }
                }
                bmpList.Add(result);
            }
            return result;
        }

        private Bitmap Negativ(Image image) //негатив
        {
            Bitmap result = new Bitmap(image);
            Rectangle rect = new Rectangle(0, 0, result.Width, result.Height);
            BitmapData bitmapData = result.LockBits(rect, ImageLockMode.ReadWrite, result.PixelFormat);
            IntPtr ptr = bitmapData.Scan0;
            int bytes = bitmapData.Stride * result.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            for (int i = 0; i < rgbValues.Length; i++)
            {
                if ((i+1) % 4 == 0) continue;
                rgbValues[i] = (byte)(255 - rgbValues[i]);
            }
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            result.UnlockBits(bitmapData);
            bmpList.Add(result);
            return result;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) //реализация фильтров 
        {
            if (bmpList.Count == 0)
            {
                MessageBox.Show("Выполните добавление изображения");
                return;
            }
            switch(comboBox2.Text)
            {
                case "ЧБ":
                    pictureBox1.Image = ChB(pictureBox1.Image);
                    break;
            }
        }

        private Bitmap ChB (Image image) //чб
        {
            Bitmap result = new Bitmap(image);
            Rectangle rect = new Rectangle(0, 0, result.Width, result.Height);
            BitmapData bitmapData = result.LockBits(rect, ImageLockMode.ReadWrite, result.PixelFormat);
            IntPtr ptr = bitmapData.Scan0;
            int bytes = bitmapData.Stride * result.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            for (int i = 0; i < rgbValues.Length; i += 4)
            {
                rgbValues[i] = rgbValues[i + 1] = rgbValues[i + 2] = (byte)((rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2])/3);
            }
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            result.UnlockBits(bitmapData);
            bmpList.Add(result);
            return result;
        }

        private void button3_Click(object sender, EventArgs e) //сохранение
        {
            if (pictureBox1.Image != null) ImSave();
            else MessageBox.Show("Выберите изображение");
        }



        private void Form3_FormClosing(object sender, FormClosingEventArgs e) //выход
        {
            if (pictureBox1.Image != null)
            {
                dr = MessageBox.Show("Сохранить изображение перед удалением?", "Внимание!", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes) //если да, то сохраняем и добавляем новое
                {
                    ImSave();
                    bmpList.Clear();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image == null)
            {
                MessageBox.Show("Выберите изображение");
                return;
            }
            if(bmpList.Count == 1)
            {
                MessageBox.Show("Изображение в начальном состоянии");
                return;
            }
            bmpList.RemoveAt(bmpList.Count - 1);
            pictureBox1.Image = bmpList.Last();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Выберите изображение");
                return;
            }
            if (bmpList.Count == 1)
            {
                MessageBox.Show("Изображение в начальном состоянии");
                return;
            }
            Bitmap bitmap = new Bitmap(bmpList.FirstOrDefault());
            bmpList.Clear();
            bmpList.Add(bitmap);
            pictureBox1.Image = bitmap;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Выберите изображение");
                return;
            }
            if (bmpList.Count == 1)
            {
                MessageBox.Show("Изображение в начальном состоянии");
                return;
            }
            Form4.begin = bmpList.FirstOrDefault();
            Form4.end = bmpList.Last();
            Form4 f4 = new Form4();
            f4.ShowDialog();
        }
    }
}
 