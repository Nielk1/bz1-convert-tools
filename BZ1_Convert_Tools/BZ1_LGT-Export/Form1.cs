using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BZ1_LGT_Export
{

    struct Zone
    {
        public byte[] Light;// = new byte[128];
    }

    public partial class Form1 : Form
    {
        List<Zone> segs = new List<Zone>();
        Bitmap imageToShow = null;
        int targetWidth = 0;
        int imageWidth = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream fileStream = openFileDialog1.OpenFile();

                {
                    saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + ".bmp";
                    saveFileDialog1.InitialDirectory = Path.GetFullPath(openFileDialog1.FileName);
                }

                {
                    targetWidth = 0;
                    imageWidth = 0;

                    pictureBox1.Image = null;
                    pictureBox1.Height = 0;
                    pictureBox1.Width = 0;
                }

                segs.Clear();

                byte[] buffer = new byte[128 * 128];
                int segments = 0;
                while (fileStream.Read(buffer, 0, 128 * 128) > 0)
                {
                    segs.Add(new Zone { Light = buffer });
                    segments++;
                    buffer = new byte[128 * 128];
                }

                numericUpDown1.Maximum = segments - 1;

                setImage((int)numericUpDown1.Value);
            }
        }

        private void setImage(int segWidth)
        {
            targetWidth = segWidth;
            timer1.Start();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            setImage((int)numericUpDown1.Value);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int segWidth = (int)e.Argument;

            int totalSegs = segs.Count - 1;

            if (totalSegs > 0)
            {
                int height = (int)Math.Ceiling(totalSegs * 1.0 / segWidth) * 128;
                Bitmap image = new Bitmap(128 * segWidth, height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

                for (int x = 0; x < totalSegs; x++)
                {
                    int xOff = (x % segWidth) * 128;
                    int yOff = (x / segWidth) * 128;
                    for (int y = 0; y < segs[x + 1].Light.Count(); y++)
                    {
                        int value = (int)segs[x + 1].Light[y];
                        image.SetPixel(xOff + (y % 128), height - 1 - (yOff + (y / 128)), Color.FromArgb(value, value, value));
                    }
                }
                imageToShow = image;
                imageWidth = segWidth;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Height = imageToShow.Height;
            pictureBox1.Width = imageToShow.Width;
            pictureBox1.Image = imageToShow;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (targetWidth != imageWidth)
            {
                if (!backgroundWorker1.IsBusy)
                    backgroundWorker1.RunWorkerAsync(targetWidth);
            }
            else
            {
                timer1.Stop();
            }
        }
    }
}
