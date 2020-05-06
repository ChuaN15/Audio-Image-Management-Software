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
    public partial class VideoForm : Form
    {
        public VideoForm()
        {
            InitializeComponent();

            //axWindowsMediaPlayer1.URL = @"C:\Users\chuan\source\repos\MultimediaDatabase\MultimediaDatabase\bin\Debug\1.mp4";
            //axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void audioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmMain().Show();
            Hide();
        }

        public class MediaFile
        {
            public string FileName { get; set; }
            public string Path { get; set; }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void VideoForm_Load(object sender, EventArgs e)
        {
            listBox1.ValueMember = "Path";
            listBox1.DisplayMember = "FileName";
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true, ValidateNames = true, Filter = "WMV|*.wmv|WAV|*.wav|MP3|*.mp3|MP4|*.mp4|MKV|*.mkv" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //Add file to play list
                    List<MediaFile> files = new List<MediaFile>();
                    foreach (string fileName in ofd.FileNames)
                    {
                        FileInfo fi = new FileInfo(fileName);
                        files.Add(new MediaFile() { FileName = Path.GetFileNameWithoutExtension(fi.FullName), Path = fi.FullName });
                    }
                    listBox1.DataSource = files;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MediaFile file = listBox1.SelectedItem as MediaFile;
            if (file != null)
            {
                axWindowsMediaPlayer1.URL = file.Path;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }
    }
}
