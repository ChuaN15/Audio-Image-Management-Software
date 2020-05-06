using EyeOpen.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SHANUAudioVedioPlayListPlayer
{
    public partial class ImageForm : Form
    {
        public ImageForm()
        {
            InitializeComponent();
            

            DataGridViewImageColumn imageCol = new DataGridViewImageColumn();
            imageCol.HeaderText = "test1";
            dataGridView1.Columns.Add(imageCol);
            imageCol.ImageLayout = DataGridViewImageCellLayout.Stretch;

            dataGridView1.RowTemplate.Height = 100;
        }

        MultimediaDatabaseEntities ent = new MultimediaDatabaseEntities();

        private delegate void ProcessImagesDelegate(FileInfo[] files);

        private delegate void SetMaximumDelegate(ProgressBar progressBar, int value);

        private delegate void UpdateOperationStatusDelegate(string format, Label label, ProgressBar progressBar, int value, DateTime startTime);

        private delegate void UpdateDataGridViewDelegate(BindingList<SimilarityImages> images, DataGridView dataGridView);

        private delegate void DeleteImageDelegate(FileInfo fileInfo);

        private delegate void ShowGridDelegate(DataGridView dataGridView);


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void loadImages()
        {
            //dataGridView1.DataSource = ent.ImageTables.ToList();

            List<ImageTable> imgList = ent.ImageTables.ToList();

            dataGridView1.Rows.Clear();

            for (int i = 0; i < imgList.Count; i++)
            {
                byte[] img = (byte[])imgList[i].Image;
                MemoryStream ms = new MemoryStream(img);
                 // will do the trick
                dataGridView1.Rows.Add(imgList[i].ID.ToString(),Image.FromStream(ms));
            }
        }

        private readonly UpdateOperationStatusDelegate updateOperationStatusDelegate = delegate (string format, System.Windows.Forms.Label label, ProgressBar progressBar, int value, DateTime startTime)
        {
            progressBar.Value = value;
            var percentage = Math.Round(((double)progressBar.Value / (double)progressBar.Maximum), 3);
            format += " {0}/{1} ({2}) Elapsed: {3} Estimated: {4}";

            var elapsed = DateTime.Now.Subtract(startTime);
            elapsed = new TimeSpan(elapsed.Days, elapsed.Hours, elapsed.Minutes, elapsed.Seconds, 0);

            var estimatedTicks = (elapsed.Ticks / value) * progressBar.Maximum;
            var estimated = new TimeSpan(estimatedTicks);
            estimated = new TimeSpan(estimated.Days, estimated.Hours, estimated.Minutes, estimated.Seconds, 0);

            label.Text = string.Format(format, progressBar.Value, progressBar.Maximum, percentage.ToString("P"), elapsed.ToString(), estimated.ToString());
        };

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private BindingList<SimilarityImages> similarityImages;

        private readonly SetMaximumDelegate setMaximumDelegate = delegate (ProgressBar progressBar, int value)
        {
            progressBar.Maximum = value;
        };

        private void ProcessImages(FileInfo[] files)
        {
            var comparableImages = new List<ComparableImage>();

            var index = 0x0;

            var operationStartTime = DateTime.Now;

            foreach (var file in files)
            {

                var comparableImage = new ComparableImage(file);
                comparableImages.Add(comparableImage);
                index++;
                Invoke(updateOperationStatusDelegate, new object[] { "Processed images", workingLabel, workingProgressBar, index, operationStartTime });
            }

            Invoke(setMaximumDelegate, new object[] { workingProgressBar, (comparableImages.Count * (comparableImages.Count - 1)) / 2 });

            index = 0;

            var similarityImagesSorted = new List<SimilarityImages>();

            operationStartTime = DateTime.Now;

            for (var i = 0; i < comparableImages.Count - 1; i++)
            {
                for (var j = i + 1; j < comparableImages.Count; j++)
                {

                    var source = comparableImages[i];
                    var destination = comparableImages[j];
                    var similarity = source.CalculateSimilarity(destination);
                    var sim = new SimilarityImages(source, destination, similarity);

                    similarityImagesSorted.Add(sim);
                    index++;

                    Invoke(updateOperationStatusDelegate, new object[] { "Compared images", workingLabel, workingProgressBar, index, operationStartTime });
                }
            }

            similarityImagesSorted.Sort();
            similarityImagesSorted.Reverse();
            similarityImages = new BindingList<SimilarityImages>(similarityImagesSorted);

            BeginInvoke(updateDataGridViewDelegate, new object[] { similarityImages, imagesDataGridView });
        }

        private readonly UpdateDataGridViewDelegate updateDataGridViewDelegate = delegate (BindingList<SimilarityImages> images, DataGridView dataGridView)
        {
            images.RaiseListChangedEvents = true;

            dataGridView.DataSource = images.Where(x => x.Source.ToString().Contains("temp") || x.Destination.ToString().Contains("temp")).ToList();

            dataGridView.Columns["Similarity"].DisplayIndex = 0;
            dataGridView.Columns["Source"].DisplayIndex = 1;
            dataGridView.Columns["Destination"].DisplayIndex = 2;

            if (images.Count > 0)
            {
                dataGridView.Rows[0].Selected = true;
            }
        };

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void ShowSelectedImages()
        {
            if (imagesDataGridView.SelectedRows.Count <= 0)
            {
                return;
            }

            var sim = (SimilarityImages)imagesDataGridView.SelectedRows[0].DataBoundItem;

            var streamSource = new System.IO.FileStream(sim.Source.File.FullName, FileMode.Open, FileAccess.Read);
            var streamDestination = new System.IO.FileStream(sim.Destination.File.FullName, FileMode.Open, FileAccess.Read);
            var source = Image.FromStream(streamSource);
            var destination = Image.FromStream(streamDestination);
            streamSource.Close();
            streamDestination.Close();

            const string InfoFormat = "Resolution: {0}x{1}\nSize: {2}kb\nFull path: {3}";
            const string ResolutionFormat = "{0} ({1}x{2})";

            sourcePictureBox.Image = source;
            sourceLabel.Text = string.Format(ResolutionFormat, "Source", source.Width, source.Height);

            destinationPictureBox.Image = destination;
            destinationLabel.Text = string.Format(ResolutionFormat, "Destination", destination.Width, destination.Height);
        }

        private void imagesDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void imagesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void imagesDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        private void audioToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog newfile = new OpenFileDialog();

            if (newfile.ShowDialog() == DialogResult.OK)
            {
                byte[] buffer = File.ReadAllBytes(newfile.FileName);
                ImageTable newimage = new ImageTable();
                newimage.Image = buffer;
                ent.ImageTables.Add(newimage);
                ent.SaveChanges();

                File.Copy(newfile.FileName, @"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Image\" + newimage.ID.ToString() + ".jpg");

            }

            loadImages();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            int id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
            var whichimage = ent.ImageTables.FirstOrDefault(x => x.ID == id);

            var picbyte = (Byte[])(whichimage.Image);
            var stream = new MemoryStream(picbyte);
            pictureBox1.Image = Image.FromStream(stream);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            var processImagesDelegate = new ProcessImagesDelegate(ProcessImages);
            var allimages = ent.ImageTables.ToList();

            OpenFileDialog newfile = new OpenFileDialog();

            if (newfile.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(@"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Image\temp.jpg"))
                {
                    File.Delete(@"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Image\temp.jpg");
                }

                File.Copy(newfile.FileName, @"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Image\temp.jpg");

                for (int i = 0; i < allimages.Count; i++)
                {
                    byte[] bytes = (byte[])allimages[i].Image;

                    MemoryStream memStream = new MemoryStream(bytes);

                    try
                    {
                        Bitmap MyImage = new Bitmap(memStream);

                        MyImage = new Bitmap(MyImage);

                        MyImage.Save(allimages[i].ID + ".jpg", ImageFormat.Jpeg);
                    }
                    catch (Exception ex)
                    {
                    }
                }


                var folder = @"C:\Users\chuan\source\repos\AudioVideoPlayer\SHANUAudioVedioPlayListPlayer\bin\Debug\Image\";

                DirectoryInfo directoryInfo;
                FileInfo[] files;
                try
                {
                    directoryInfo = new DirectoryInfo(folder);

                    files = directoryInfo.GetFiles("*.jpg", SearchOption.AllDirectories);
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show("Path not valid.", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Path not valid.", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                processImagesDelegate.BeginInvoke(files, null, null);
            }
        }

        private void imagesDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {

        }

        private void imagesDataGridView_SelectionChanged_1(object sender, EventArgs e)
        {
            ShowSelectedImages();
        }

        private void ImageForm_Load(object sender, EventArgs e)
        {
            loadImages();
        }

        private void imagesDataGridView_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void workingLabel_Click(object sender, EventArgs e)
        {

        }

        private void workingProgressBar_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
            var whichimage = ent.ImageTables.FirstOrDefault(x => x.ID == id);
            
            new ImageEditForm(whichimage.ID).ShowDialog();

            ent = new MultimediaDatabaseEntities();
            ImageForm_Load(null, null);

        }
    }
}
