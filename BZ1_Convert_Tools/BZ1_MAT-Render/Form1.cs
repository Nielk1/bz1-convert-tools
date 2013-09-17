using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using StumDE.Misc;
using System.Drawing.Imaging;

namespace BZ1_MAT_Render
{
    public partial class Form1 : Form
    {
        Color[] colors = {
            Color.FromArgb(0x00,0x00,0x88),
            Color.FromArgb(0x00,0x00,0xFF),
            Color.FromArgb(0x00,0x88,0x00),
            Color.FromArgb(0x00,0x88,0x88),
            Color.FromArgb(0x00,0x88,0xFF),
            Color.FromArgb(0x00,0xFF,0x00),
            Color.FromArgb(0x00,0xFF,0x88),
            Color.FromArgb(0x00,0xFF,0xFF),
            Color.FromArgb(0x88,0x00,0x00),
            Color.FromArgb(0x88,0x00,0x88),
            Color.FromArgb(0x88,0x00,0xFF),
            Color.FromArgb(0x88,0x88,0x00),
            Color.FromArgb(0x88,0x88,0x88),
            Color.FromArgb(0x88,0x88,0xFF),
            Color.FromArgb(0x88,0xFF,0x00),
            Color.FromArgb(0x88,0xFF,0x88),
            Color.FromArgb(0x88,0xFF,0xFF),
            Color.FromArgb(0xFF,0x00,0x00),
            Color.FromArgb(0xFF,0x00,0x88),
            Color.FromArgb(0xFF,0x00,0xFF),
            Color.FromArgb(0xFF,0x88,0x00),
            Color.FromArgb(0xFF,0x88,0x88),
            Color.FromArgb(0xFF,0x88,0xFF),
            Color.FromArgb(0xFF,0xFF,0x00),
            Color.FromArgb(0xFF,0xFF,0x88)
        };

        Dictionary<string, string>[] textures;

        public Form1()
        {
            InitializeComponent();
            textures = new Dictionary<string, string>[16];
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream fileStream = openFileDialog1.OpenFile();

                MatFile mat = new MatFile(fileStream);

                //int width = 32;
                int width = 256;// 256;
                int zoneNum = 1;

                int subdivisions = 2;

                foreach (MatZone zone in mat.zoneList)
                {
                //    Bitmap bmp = new Bitmap(width * 64, width * 64, PixelFormat.Format16bppRgb565);
                //    Graphics g = Graphics.FromImage(bmp);
                //    for (int y = 0; y < 64; y++)
                //    {
                //        for (int x = 0; x < 64; x++)
                //        {
                //            int mix = zone.GetMix(x, y);
                //            int baseT = zone.GetBase(x,y);
                //            int nextT = zone.GetNext(x,y);
                //            int var = zone.GetVariant(x, y);
                //            Bitmap tileImage = renderTexture(width, mix, baseT, nextT, var);
                //            g.DrawImage(tileImage, x * width, (64 - y - 1) * width);
                //        }
                //    }
                //    string filename = openFileDialog1.FileName;
                //    filename = Path.GetFileNameWithoutExtension(filename);
                //    saveFileDialog1.FileName = filename + "_" + zoneNum + ".bmp";
                //    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                //    {
                //        bmp.Save(saveFileDialog1.FileName);
                //    }
                //    zoneNum++;

                    int subDiv = 1;

                    for (int yO = 0; yO < subdivisions; yO++)
                    {
                        for (int xO = 0; xO < subdivisions; xO++)
                        {
                            Bitmap bmp = new Bitmap(width * 64 / subdivisions, width * 64 / subdivisions, PixelFormat.Format16bppRgb565);
                            Graphics g = Graphics.FromImage(bmp);
                            for (int yI = 0; yI < (64 / subdivisions); yI++)
                            {
                                int y = yI + ((64 / subdivisions) * yO);
                                for (int xI = 0; xI < (64 / subdivisions); xI++)
                                {
                                    int x = xI + ((64 / subdivisions) * xO);
                                    int mix = zone.GetMix(x, y);
                                    int baseT = zone.GetBase(x, y);
                                    int nextT = zone.GetNext(x, y);
                                    int var = zone.GetVariant(x, y);
                                    Bitmap tileImage = renderTexture(width, mix, baseT, nextT, var);
                                    g.DrawImage(tileImage, xI * width, ((64 / subdivisions) - yI - 1) * width);
                                    tileImage.Dispose();
                                }
                            }
                            g.Dispose();
                            string filename = openFileDialog1.FileName;
                            filename = Path.GetFileNameWithoutExtension(filename);
                            //saveFileDialog1.FileName = filename + "_" + zoneNum + "_" + subDiv + ".png";
                            //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            //{
                            //    bmp.Save(saveFileDialog1.FileName);
                            //}
                            bmp.Save(Path.GetDirectoryName(openFileDialog1.FileName) + Path.DirectorySeparatorChar + filename + "_" + zoneNum + "_" + subDiv + ".png", ImageFormat.Png);
                            bmp.Dispose();
                            subDiv++;
                        }
                    }
                    zoneNum++;
                }
            }
        }

        private Bitmap renderTexture(int width, int mix, int baseV, int nextV, int var)
        {
            if (textures[baseV] != null && textures[nextV] != null)
            {
                //Bitmap bmp = new Bitmap(width, width);
                //Bitmap source = new Bitmap(textures[baseV]

                Bitmap source;
                Bitmap output = new Bitmap(width, width, PixelFormat.Format16bppRgb565);

                string fname = string.Empty;

                try
                {
                    if (baseV != nextV)
                    {
                        if (mix >= 0 & mix < 8)
                        {
                            string keyname = "capto" + nextV + "_" + ("abcd"[var]) + "0";
                            if(!textures[baseV].ContainsKey(keyname))
                                keyname = "capto" + nextV + "_a0";
                            fname = textures[baseV][keyname];
                        }
                        else
                        {
                            string keyname = "diagonalto" + nextV + "_" + ("abcd"[var]) + "0";
                            if (!textures[baseV].ContainsKey(keyname))
                                keyname = "diagonalto" + nextV + "_a0";
                            fname = textures[baseV][keyname];
                        }
                    }
                    else
                    {
                        string keyname = "solid" + ("abcd"[var]) + "0";
                        if (!textures[baseV].ContainsKey(keyname))
                            keyname = "solida0";
                        fname = textures[baseV][keyname];
                    }


                    fname = AppDomain.CurrentDomain.BaseDirectory + fname.Trim() + ".bmp";


                    source = new Bitmap(fname);

                    Graphics g = Graphics.FromImage(output);

                    g.DrawImage(source, 0, 0, width, width);
                    source.Dispose();

                    switch (mix)
                    {
                        case 0:
                        case 8:
                            break;
                        case 1:
                        case 9:
                            output.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case 2:
                        case 10:
                            output.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 3:
                        case 11:
                            output.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 4:
                        case 15:
                            output.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 5:
                        case 12:
                            output.RotateFlip(RotateFlipType.Rotate270FlipY);
                            break;
                        case 6:
                        case 13:
                            output.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case 7:
                        case 14:
                            output.RotateFlip(RotateFlipType.Rotate90FlipY);
                            break;
                    }

                    g.Dispose();

                    return output;
                }
                catch (Exception)
                {
                    Console.WriteLine(fname);
                    return renderPlaceholder(width, mix, baseV, nextV, var);
                }
            }
            return renderPlaceholder(width, mix, baseV, nextV, var);
        }

        private Bitmap renderPlaceholder(int width, int mix, int baseV, int nextV, int var)
        {
            Bitmap bmp = new Bitmap(width, width, PixelFormat.Format16bppRgb565);

            Graphics g = Graphics.FromImage(bmp);
            Brush blackBrush = new SolidBrush(Color.Black);
            Brush redBrush = new SolidBrush(Color.Red);
            Pen whitePen = new Pen(Color.White, 2);
            //Brush whiteBrush = new SolidBrush(Color.White);

            //Brush baseBrush = new SolidBrush(Color.FromArgb(0, 0, (int)(baseV / 16.0 * 255)));
            //Brush nextBrush = new SolidBrush(Color.FromArgb(0, 0, (int)(nextV / 16.0 * 255)));

            Brush baseBrush = new SolidBrush(colors[baseV]);
            Brush nextBrush = new SolidBrush(colors[nextV]);

            g.FillRectangle(baseBrush, 0, 0, width, width);

            if (baseV != nextV)
            {
                if (mix >= 0 & mix < 8)
                {
                    g.FillEllipse(nextBrush, (int)((width / 32.0) * -7), width - (int)((width / 32.0) * 8), (int)((width / 32.0) * 44), (int)((width / 32.0) * 44));
                    g.DrawEllipse(whitePen, (int)((width / 32.0) * -7), width - (int)((width / 32.0) * 8), (int)((width / 32.0) * 44), (int)((width / 32.0) * 44));
                }
                else
                {
                    g.FillPolygon(nextBrush, new Point[] { new Point(0, width), new Point(width, 0), new Point(width, width) });
                    g.DrawLine(whitePen, 0, width, width, 0);
                }

                g.FillEllipse(redBrush, (int)((width / 32.0) * 4), (int)((width / 32.0) * 26), 6, 6);
            }

            g.DrawString("" + ("ABCD"[var]), new Font("Arial", 16), redBrush, (int)((width / 32.0) * 8) - 2, (int)((width / 32.0) * 8) - 2);

            switch (mix)
            {
                case 0:
                case 8:
                    break;
                case 1:
                case 9:
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case 2:
                case 10:
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                case 11:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 4:
                case 15:
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 5:
                case 12:
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipY);
                    break;
                case 6:
                case 13:
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
                    break;
                case 7:
                case 14:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipY);
                    break;
            }

            /*if (mix < 8)
            {
                g.DrawEllipse(whitePen, -7, 32 - 8, 44, 44);
            }
            else
            {
                g.DrawLine(whitePen, 0, 32, 32, 0);
            }*/

            g.Dispose();

            return bmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = renderPlaceholder(32, 12, 0, 15, 0);
        }

        private void openIniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                IniFile ini = new IniFile(openFileDialog2.FileName);
                for (int t = 0; t < 16; t++)
                {
                    Dictionary<string,string> section = ini.GetSection("TextureType" + t);
                    if (section.Count > 0)
                    {
                        textures[t] = section.Select(dr => new { key = dr.Key, value = Path.GetFileNameWithoutExtension(dr.Value) }).ToDictionary(dr => dr.key, dy => dy.value);

                    }
                    else
                    {
                        textures[t] = null;
                    }
                }
            }
        }
    }
}
