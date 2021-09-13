using Alea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGPU.Labs.Interface;
using System.Drawing;
using Alea.Parallel;
using System.Drawing.Imaging;
using Alea.CSharp;
using System.Security.Cryptography;
using Alea.FSharp;
using System.IO;

namespace TestGPU.Labs
{
    class Lab3 : ILab
    {
        public void Execute()
        {
            Console.WriteLine("Lab 3 - Размытие изображения на CUDA");

            Gpu gpu = Gpu.Default;

            Image image = Image.FromFile($@"{ Directory.GetCurrentDirectory() }\input\Lab3Cat.jpg");
            var bmImage = new Bitmap(image);

            var pixelMap = GetPixelMap(bmImage);

            int[,] resultR, resultG, resultB;
            int w = bmImage.Width;
            int h = bmImage.Height;

            using (var r_gpu = gpu.AllocateDevice<int>(w, h))
            using (var g_gpu = gpu.AllocateDevice<int>(w, h))
            using (var b_gpu = gpu.AllocateDevice<int>(w, h))
            {
                var ptrR = r_gpu.Ptr;
                var ptrG = g_gpu.Ptr;
                var ptrB = b_gpu.Ptr;
                var pitchR = r_gpu.PitchInElements.ToInt32();
                var pitchG = g_gpu.PitchInElements.ToInt32();
                var pitchB = b_gpu.PitchInElements.ToInt32();

                gpu.For(0, w, i =>
                {
                    for (int j = 0; j < h; j++)
                    {
                        var newPixel = BlurPixel(pixelMap, i, j, w, h);
                        ptrR[i * pitchR + j] = newPixel.r;
                        ptrG[i * pitchG + j] = newPixel.g;
                        ptrB[i * pitchB + j] = newPixel.b;
                    }
                });
                resultR = Gpu.Copy2DToHost(r_gpu);
                resultG = Gpu.Copy2DToHost(g_gpu);
                resultB = Gpu.Copy2DToHost(b_gpu);
            }

            bmImage = SetNewPixelMap(bmImage, resultR, resultG, resultB);

            bmImage.Save($@"{ Directory.GetCurrentDirectory() }\output\Lab3CatConverted.jpg", ImageFormat.Jpeg);

            Console.WriteLine("The end");
            Console.ReadKey();
        }

        private RGB BlurPixel(RGB[,] pixelMap, int w, int h, int width, int height)
        {
            double r = 0;
            double g = 0;
            double b = 0;
            var counter = 0;
            var radius = 5;

            for (int i = w - radius; i < w + radius; i++)
            {
                if (i >= 0 && i < width)
                    for (int j = h - radius; j < h + radius; j++)
                    {
                        if (j >= 0 && j < height)
                        {
                            var cur = pixelMap[i, j];

                            r += cur.r;
                            g += cur.g;
                            b += cur.b;
                            counter++;
                        }
                    }
            }

            return new RGB
            {
                r = (int)Math.Round(r / counter),
                g = (int)Math.Round(g / counter),
                b = (int)Math.Round(b / counter)
            };
        }

        private Bitmap SetNewPixelMap(Bitmap image, int[,] newPixelMapR, int[,] newPixelMapG, int[,] newPixelMapB)
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    var r = newPixelMapR[i, j];
                    var g = newPixelMapG[i, j];
                    var b = newPixelMapB[i, j];
                    image.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }

            return image;
        }

        private RGB[,] GetPixelMap(Bitmap image)
        {
            var pixelMap = new RGB[image.Width, image.Height];

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    pixelMap[i, j] = new RGB
                    {
                        r = image.GetPixel(i, j).R,
                        g = image.GetPixel(i, j).G,
                        b = image.GetPixel(i, j).B,
                    };
                }
            }

            return pixelMap;
        }     

        private struct RGB
        {
            public int r { get; set; }
            public int g { get; set; }
            public int b { get; set; }
        }
    }
}
