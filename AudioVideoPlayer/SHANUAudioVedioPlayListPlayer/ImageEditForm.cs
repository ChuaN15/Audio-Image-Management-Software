using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SHANUAudioVedioPlayListPlayer
{
    public partial class ImageEditForm : Form
    {
        int id;
        MultimediaDatabaseEntities ent = new MultimediaDatabaseEntities();

        public ImageEditForm(int id)
        {
            InitializeComponent();
            this.id = id;

            var whichid = ent.ImageTables.FirstOrDefault(X => X.ID == id);

            label5.Text = whichid.ID.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog newfile = new OpenFileDialog();

            if (newfile.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = newfile.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var whichid = ent.ImageTables.FirstOrDefault(X => X.ID == id);
            ent.ImageTables.Remove(whichid);
            ent.SaveChanges();

            File.Delete(@"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Image\" + whichid.ID.ToString() + ".jpg");

            MessageBox.Show("Image has been removed successfully!");
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var whichid = ent.ImageTables.FirstOrDefault(X => X.ID == id);

                byte[] buffer = File.ReadAllBytes(textBox2.Text);
            whichid.Image = buffer;
                ent.SaveChanges();

            File.Delete(@"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Image\" + whichid.ID.ToString() + ".jpg");
            File.Copy(textBox2.Text, @"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Image\" + whichid.ID.ToString() + ".jpg");

        MessageBox.Show("Image has been updated successfully!");
            this.Hide();
        }
    }
}
