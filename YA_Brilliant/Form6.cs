using System;
using System.Text;
using System.Windows.Forms;
using VkNet;
using VkNet.Model;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using System.Net;
using System.Drawing;

namespace YA_Brilliant
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }
        
        static public string path = "";

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "" || textBox2.Text =="" || textBox3.Text == "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            try
            {
                var api = new VkApi();
                api.Authorize(new ApiAuthParams
                {
                    ApplicationId = 7489377,
                    Login = textBox1.Text,
                    Password = textBox2.Text,
                    Settings = Settings.All
                });
                int AlbumID = Convert.ToInt32(textBox3.Text);
                var uploadServer = api.Photo.GetUploadServer(AlbumID);
                var wc = new WebClient();
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
                var photos = api.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = AlbumID
                });
                Form3.import = true;
                this.Hide();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Form3.import = false;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Blue;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Black;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3.import = false;
            this.Hide();
        }
    }
}
