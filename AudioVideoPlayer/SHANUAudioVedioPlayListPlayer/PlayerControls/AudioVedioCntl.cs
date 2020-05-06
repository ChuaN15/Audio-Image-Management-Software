using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WMPLib;
//Author  : Syed Shanu
//Date    : 2014-11-16
//Description : Shanu Audio/Video Player
namespace SHANUAudioVedioPlayListPlayer.PlayerControls
{
    public partial class AudioVedioCntl : UserControl
    {
        #region Members
        int Startindex = 0;
        string[] FileName, FilePath;
           Boolean playnext = false;
        MultimediaDatabaseEntities ent = new MultimediaDatabaseEntities();

        #endregion

           #region ControlLoad
           public AudioVedioCntl()
        {
            InitializeComponent();
        }


        private void AudioVedioCntl_Load(object sender, EventArgs e)
        {
            Startindex = 0;
            playnext = false;
            stopPlayer();

            loadImages();
        }

           #endregion

        #region Method
        // to stop the player
        public void stopPlayer()
        {
            WindowsMediaPlayer.Ctlcontrols.stop();
        }

        // To Play the player
        public void playfile(int playlistindex)
        {
            if (listBox1.Items.Count <= 0)
            { return; }
            if (playlistindex < 0)
            {
                return;
            }
            WindowsMediaPlayer.settings.autoStart = true;
            //axWindowsMediaPlayer1.URL = @"C:\Users\chuan\source\repos\MultimediaDatabase\MultimediaDatabase\bin\Debug\1.mp4";
            WindowsMediaPlayer.URL = FilePath[playlistindex];
            WindowsMediaPlayer.Ctlcontrols.next();
            WindowsMediaPlayer.Ctlcontrols.play();
        }
        #endregion

        #region Events
        #region Load Audio or Video Files

        //Load Audio or Vedio files 
        private void loadImages()
        {
            listBox1.Items.Clear();

            Startindex = 0;
            playnext = false;
            string pathName = @"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Audio\";

            var allAudio = ent.AudioTables.ToList();

            for (int i = 0; i < allAudio.Count; i++)
                {
                    listBox1.Items.Add(allAudio[i].Name);
                FileName = allAudio.Select(x => x.Name).ToArray();
                FilePath = allAudio.Select(x => pathName + x.ID.ToString() + ".mp3").ToArray();
            }


                Startindex = 0;
                playfile(0);
        }
        #endregion

        #region Selected File Play 

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Startindex = listBox1.SelectedIndex;
            playfile(Startindex);
        }

        #endregion
        #region Timer event to check for the End of song or video and play the next song or video
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (playnext == true)
            {
                playnext = false;
                playfile(Startindex);

            }
        }
        #endregion

        #region Play the First Song from the Play List
        private void btnfirst_Click(object sender, EventArgs e)
        {
            Startindex = 0;
            playfile(0);
        }
        #endregion
        #region Play the selected Song from the Play List

        private void btnPlay_Click(object sender, EventArgs e)
        {
               WindowsMediaPlayer.Ctlcontrols.play();
        }
        #endregion

        #region Pause the selected Song from the Play List

        private void btnpause_Click(object sender, EventArgs e)
        {
              WindowsMediaPlayer.Ctlcontrols.pause();
        }
        #endregion

        #region Stop the selected Song from the Play List
        private void btnStop_Click(object sender, EventArgs e)
        {
            stopPlayer();
        }
        #endregion

        #region Play the previous song from the selected Song from  Play List
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (Startindex > 0)
            {
                Startindex = Startindex - 1;
            }

            playfile(Startindex);
        }
        #endregion

        #region Play the Next  song from the selected Song from  Play List
        private void btnNext_Click(object sender, EventArgs e)
        {
          

             if (Startindex == listBox1.Items.Count-1)
            {
                Startindex = listBox1.Items.Count-1;
            }
            else if (Startindex < listBox1.Items.Count)
            {
                Startindex = Startindex + 1;

            }
            
            playfile(Startindex);
        }
        #endregion

        #region Play the Last song from the  Play List
        private void btnLast_Click(object sender, EventArgs e)
        {
            Startindex = listBox1.Items.Count-1;
            playfile(Startindex);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            OpenFileDialog newfile = new OpenFileDialog();

            if (newfile.ShowDialog() == DialogResult.OK)
            {
                byte[] buffer = File.ReadAllBytes(newfile.FileName);
                AudioTable newimage = new AudioTable();
                newimage.Audio = buffer;
                newimage.Name = newfile.SafeFileName;
                newimage.DateTime = DateTime.Now;
                ent.AudioTables.Add(newimage);
                ent.SaveChanges();

                File.Copy(newfile.FileName, @"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Audio\" + newimage.ID.ToString() + ".mp3");
            }

            loadImages();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Startindex = listBox1.SelectedIndex;
            var allAudio = ent.AudioTables.ToList();

            new EditForm(allAudio[Startindex].ID).ShowDialog();

            ent = new MultimediaDatabaseEntities();
            AudioVedioCntl_Load(null,null);
        }
        #endregion

        #region This is Windows Media Player Event which we can used to fidn the status of the player and do our actions.
        private void WindowsMediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            int statuschk = e.newState;  // here the Status return the windows Media Player status where the 8 is the Song or Vedio is completed the playing .

            // Now here i check if the song is completed then i Increment to play the next song

            if (statuschk == 8)
            {
                statuschk = e.newState;

                if (Startindex == listBox1.Items.Count - 1)
                {
                    Startindex = 0;
                }
                else if (Startindex >= 0 && Startindex < listBox1.Items.Count - 1)
                {
                    Startindex = Startindex + 1;

                }
                playnext = true;
            }



        }
         #endregion
        #endregion

    }
}
