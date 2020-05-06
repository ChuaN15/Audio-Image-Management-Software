using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//Author  : Syed Shanu
//Date    : 2014-11-16
//Description : Shanu Audio/Video Player
namespace SHANUAudioVedioPlayListPlayer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            
            objAudioVideo = new PlayerControls.AudioVedioCntl();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadPlayerControl(0);
        }
        
        SHANUAudioVedioPlayListPlayer.PlayerControls.AudioVedioCntl objAudioVideo;

        private void LoadPlayerControl(int playerType)
        {
            pnlMain.Controls.Clear();


            

            if (playerType == 0)
            {

                pnlMain.Controls.Add(objAudioVideo);
                objAudioVideo.Dock = DockStyle.Fill;
            }
            else
            {
                objAudioVideo.stopPlayer();
            }

                

                

        }
        private void btnfirst_Click(object sender, EventArgs e)
        {
            LoadPlayerControl(0);
        }

        private void btnYouTubePlayer_Click(object sender, EventArgs e)
        {
            LoadPlayerControl(1);
        }

        private void imagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            objAudioVideo.stopPlayer();

            new ImageForm().Show();
            Hide();
        }

        private void audioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadPlayerControl(0);
        }

        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            objAudioVideo.stopPlayer();

            new VideoForm().Show();
            Hide();
        }
    }
}
