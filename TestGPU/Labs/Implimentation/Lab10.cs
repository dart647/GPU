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
using System.Diagnostics;

namespace TestGPU.Labs
{
    class Lab10 : ILab
    {
        public void Execute()
        {
            Console.WriteLine("Lab 10 - Свертка");

            Gpu gpu = Gpu.Default;
            var radious = 5;

            Image image = Image.FromFile($@"{ Directory.GetCurrentDirectory() }\input\Lab10Castle.jpg");
            var bmImage = new Bitmap(image);

            var pixelMap = GetPixelMap(bmImage);
            var pixelMask = GetPixelMask(radious);

            ShowMask(pixelMask, radious);

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
                        var newPixel = SetMaskToPixel(pixelMap, pixelMask, i, j, w, h, radious);
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

            bmImage.Save($@"{ Directory.GetCurrentDirectory() }\output\Lab10CastleConverted.jpg", ImageFormat.Jpeg);

            Console.WriteLine("The end");
            Console.ReadKey();
        }

        private RGB SetMaskToPixel(RGB[,] pixelMap, RGBMask[,] pixelMask, int w, int h, int width, int height, double radious)
        {
            double r = 0;
            double g = 0;
            double b = 0;
            var counter = 0;
            var rad = (int)Math.Floor(radious / 2);

            for (int i = w - rad, k = 0; i < w + rad; i++, k++)
            {
                if (i >= 0 && i < width)
                    for (int j = h - rad, d = 0; j < h + rad; j++, d++)
                    {
                        if (j >= 0 && j < height)
                        {
                            var pixel = pixelMap[i, j];
                            var mask = pixelMask[k, d];
                            r += pixel.r * mask.r;
                            g += pixel.g * mask.g;
                            b += pixel.b * mask.b;
                            counter++;
                        }
                    }
            }

            return new RGB
            {
                r = (int)Math.Floor(r / counter),
                g = (int)Math.Floor(g / counter),
                b = (int)Math.Floor(b / counter)
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

        private RGBMask[,] GetPixelMask(int radious)
        {
            var pixelMask = new RGBMask[radious, radious];

            var rand = new Random();

            for (int i = 0; i < radious; i++)
            {
                for (int j = 0; j < radious; j++)
                {
                    pixelMask[i, j] = new RGBMask
                    {
                        r = rand.NextDouble(),
                        g = rand.NextDouble(),
                        b = rand.NextDouble(),
                    };
                }
            }

            return pixelMask;
        }

        private void ShowMask(RGBMask[,] mask, int radious)
        {
            Console.WriteLine("Маска:");
            for (int i = 0; i < radious; i++)
            {
                for (int j = 0; j < radious; j++)
                {
                    Console.Write($"({mask[i, j].r:0.00}, {mask[i, j].g:0.00}, {mask[i, j].b:0.00}) ");
                }
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private struct RGB
        {
            public int r { get; set; }
            public int g { get; set; }
            public int b { get; set; }
        }
        private struct RGBMask
        {
            public double r { get; set; }
            public double g { get; set; }
            public double b { get; set; }
        }

    }
}
