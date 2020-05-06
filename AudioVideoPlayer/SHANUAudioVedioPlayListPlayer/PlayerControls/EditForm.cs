using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SHANUAudioVedioPlayListPlayer.PlayerControls
{
    public partial class EditForm : Form
    {
        int id;
        MultimediaDatabaseEntities ent = new MultimediaDatabaseEntities();

        public EditForm(int id)
        {
            InitializeComponent();
            this.id = id;

            var whichid = ent.AudioTables.FirstOrDefault(X => X.ID == id);

            textBox1.Text = whichid.Name;
            label4.Text = whichid.DateTime.ToShortDateString();
        }

        string safe = "";

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var whichid = ent.AudioTables.FirstOrDefault(X => X.ID == id);

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                whichid.Name = textBox1.Text;
                ent.SaveChanges();
            }
            else
            {
                byte[] buffer = File.ReadAllBytes(textBox2.Text);
                whichid.Audio = buffer;
                whichid.DateTime = DateTime.Now;
                ent.SaveChanges();

                File.Delete(@"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Audio\" + whichid.ID.ToString() + ".mp3");
                File.Copy(textBox2.Text, @"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Audio\" + whichid.ID.ToString() + ".mp3");
            }

            MessageBox.Show("Audio has been updated successfully!");
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var whichid = ent.AudioTables.FirstOrDefault(X => X.ID == id);
            ent.AudioTables.Remove(whichid);
            ent.SaveChanges();

            File.Delete(@"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Audio\" + whichid.ID.ToString() + ".mp3");

            MessageBox.Show("Audio has been removed successfully!");
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog newfile = new OpenFileDialog();

            if (newfile.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = newfile.FileName;
                safe = newfile.SafeFileName;
            }
        }
    }
}
