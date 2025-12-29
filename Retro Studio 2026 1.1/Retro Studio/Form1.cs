using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Retro_Studio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        Bitmap sourceImage;   // Orijinal
        Bitmap baseImage;     // Resize sonrası
        Bitmap workingImage;  // Anlık filtre


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.png;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                sourceImage = new Bitmap(ofd.FileName);
                baseImage = (Bitmap)sourceImage.Clone();
                workingImage = (Bitmap)baseImage.Clone();

                pictureBox1.BackgroundImage = workingImage;
                pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            }
        }


        Bitmap ResizeImage(Bitmap img, int width, int height)
        {
            Bitmap resized = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.DrawImage(img, 0, 0, width, height);
            }
            return resized;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (baseImage == null)
            {
                MessageBox.Show("Upload an image!");
                return;
            }
            baseImage = ResizeImage(
                baseImage,
                (int)numericUpDown1.Value,
                (int)numericUpDown2.Value
            );

            workingImage = (Bitmap)baseImage.Clone();
            pictureBox1.BackgroundImage = workingImage;
        }



        private void button3_Click(object sender, EventArgs e)
        {
            if (workingImage == null) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Image";
            sfd.Filter =
                "PNG Image (*.png)|*.png|" +
                "JPEG Image (*.jpg)|*.jpg|" +
                "Bitmap Image (*.bmp)|*.bmp";
            sfd.FilterIndex = 1;
            sfd.AddExtension = true;
            sfd.OverwritePrompt = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (Bitmap saveBmp = new Bitmap(workingImage))
                {
                    switch (sfd.FilterIndex)
                    {
                        case 1:
                            saveBmp.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case 2:
                            saveBmp.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        case 3:
                            saveBmp.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                    }
                }
            }
        }



        Bitmap ReduceColors(Bitmap img, int levels)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color c = img.GetPixel(x, y);

                    // TAM ŞEFFAFSA → AYNEN KOPYALA
                    if (c.A == 0)
                    {
                        bmp.SetPixel(x, y, Color.Transparent);
                        continue;
                    }

                    // Siyah-beyaz (case 1)
                    if (levels == 0)
                    {
                        int gray = (c.R + c.G + c.B) / 3;
                        bmp.SetPixel(x, y, Color.FromArgb(c.A, gray, gray, gray));
                        continue;
                    }

                    int step = 256 / levels;

                    int r = (c.R / step) * step;
                    int g = (c.G / step) * step;
                    int b = (c.B / step) * step;

                    // 🔥 ALPHA KORUNUYOR
                    bmp.SetPixel(x, y, Color.FromArgb(c.A, r, g, b));
                }
            }
            return bmp;
        }


        int GetLevelsFromTrackbar(int value)
        {
            switch (value)
            {
                case 1: return 0;
                case 2: return 2;
                case 3: return 4;
                case 4: return 8;
                case 5: return 16;
                case 6: return 32;
                case 7: return 64;
                default: return 64;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (baseImage == null)
            {
                MessageBox.Show("Upload an image!");
                return;
            }

            int levels = GetLevelsFromTrackbar(trackBar1.Value);

            workingImage = ReduceColors(baseImage, levels);

            pictureBox1.BackgroundImage = workingImage;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (sourceImage == null) return;

            baseImage = (Bitmap)sourceImage.Clone();
            workingImage = (Bitmap)baseImage.Clone();
            pictureBox1.BackgroundImage = workingImage;
        }
    }
}
