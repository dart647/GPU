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
using System.Numerics;
using System.Threading;

namespace TestGPU.Labs
{
    class Lab9 : ILab
    {
        public void Execute()
        {
            Console.WriteLine("Lab 9 - Разностная схема");

            Gpu gpu = Gpu.Default;

            var vector = SetVector();

            var heigth = (int)vector.X;
            var width = (int)vector.Y;
            var depth = (int)vector.Z;

            var template = SetTemplate(heigth, width, depth);
            var result = new int[template.Length];

            ShowTemplate(template);

            using (var t_gpu = gpu.AllocateDevice(template))
            using (var result_gpu = gpu.AllocateDevice<int>(heigth * width * depth))
            {
                var t_ptr = t_gpu.Ptr;
                var result_ptr = result_gpu.Ptr;

                gpu.For(1, heigth - 1, h =>
                {
                    for (int w = 1; w < width - 1; w++)
                    {
                        for (int d = 1; d < depth - 1; d++)
                        {

                            var res = 
                            t_ptr[GetCellNumber(h, w, d + 1, width, depth)] + 
                            t_ptr[GetCellNumber(h, w, d - 1, width, depth)] + 
                            t_ptr[GetCellNumber(h, w + 1, d, width, depth)] +
                            t_ptr[GetCellNumber(h, w - 1, d, width, depth)] + 
                            t_ptr[GetCellNumber(h + 1, w, d, width, depth)] + 
                            t_ptr[GetCellNumber(h - 1, w, d, width, depth)] -
                            6 * t_ptr[GetCellNumber(h, w, d + 1, width, depth)];

                            result_ptr[GetCellNumber(h, w, d, width, depth)] = Clamp(res, 0, 255);
                        }
                    }
                });

                result = Gpu.CopyToHost(result_gpu);
            }

            ShowTemplate(result);

            Console.WriteLine("The end");
            Console.ReadKey();
        }

        private Vector3 SetVector()
        {
            var rand = new Random();
            var vector = new Vector3
            {
                X = rand.Next(3, 15),
                Y = rand.Next(3, 15),
                Z = rand.Next(3, 15),
            };
            Console.WriteLine($"Высота - {vector.X}, Ширина - {vector.Y}, Глубина - {vector.Z}");

            return vector;
        }

        private int[] SetTemplate(int h, int w, int d)
        {
            var rand = new Random();

            var template = new int[h * w * d];

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    for (int k = 0; k < d; k++)
                    {
                        template[(i * w + j) * d + k] = (i == 0 || j == 0 || k == 0) ||
                            (i == h - 1) || (j == w - 1) || (k == d - 1) ?
                            0 : rand.Next(255);
                    }
                }
            }

            return template;
        }

        private void ShowTemplate(int[] template)
        {
            var counter = 0;
            Console.WriteLine();
            Console.WriteLine();
            foreach (var item in template)
            {
                Console.Write($"{item}\t");

                if(counter % 15 == 0)
                    Console.WriteLine();

                counter++;
            }
            Console.WriteLine();
        }

        private int Clamp(int val, int start, int end)
        {
            return Math.Max(Math.Min(val, end), start);
        }

        private int GetCellNumber(int i, int j, int k, int width, int depth)
        {
            return (i * width + j) * depth + k;
        }
    }
}
