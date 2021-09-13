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

namespace TestGPU.Labs
{
    class Lab2 : ILab
    {
        public void Execute()
        {
            Console.WriteLine("Lab 2 - Перевод цветного изображения в градации серого на CUDA");

            Gpu gpu = Gpu.Default;

            Image image = Image.FromFile(@"D:\Work\Education\Заочка\РиПВ\TestGPU\TestGPU\Labs\Implimentation\Lab2Space.jpg");
            var bmImage = new Bitmap(image);

            var pixelMap = GetPixelMap(bmImage);
            int[,] result;
            int w = bmImage.Width;
            int h = bmImage.Height;

            using (var m_gpu = gpu.AllocateDevice<int>(w, h))
            {
                var ptr = m_gpu.Ptr;
                var pitch = m_gpu.PitchInElements.ToInt32();
                gpu.For(0, w, i =>
                {
                    for (int j = 0; j < h; j++)
                    {
                        ptr[i * pitch + j] = GrayFilter(pixelMap[i, j]);
                    }
                });
                result = Gpu.Copy2DToHost(m_gpu);
            }

            bmImage = SetNewPixelMap(bmImage, result);

            bmImage.Save(@"D:\Work\Education\Заочка\РиПВ\TestGPU\TestGPU\Labs\Implimentation\Lab2SpaceConverted.jpg", ImageFormat.Jpeg);

            Console.WriteLine("The end");
            Console.ReadKey();
        }

        private Bitmap SetNewPixelMap(Bitmap image, int[,] newPixelMap)
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    var gray = newPixelMap[i, j];
                    image.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
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

        private int GrayFilter(RGB color)
        {
            return (int)Math.Round(0.21 * color.r + 0.71 * color.g + 0.07 * color.b);
        }

        private struct RGB
        {
            public int r { get; set; }
            public int g { get; set; }
            public int b { get; set; }
        }
    }
}
