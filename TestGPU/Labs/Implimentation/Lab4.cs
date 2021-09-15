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

namespace TestGPU.Labs
{
    class Lab4 : ILab
    {
        public void Execute()
        {
            Console.WriteLine("Lab 4 - Сложение векторов на CUDA");

            Gpu gpu = Gpu.Default;

            var vectors = SetVectors();
            var length = vectors.Length;
            var resultVector = new Vector2();

            using (var vectors_x_gpu = gpu.AllocateDevice(vectors.Select(v => v.X).ToArray()))
            using (var vectors_y_gpu = gpu.AllocateDevice(vectors.Select(v => v.Y).ToArray()))
            using (var result_x_gpu = gpu.AllocateDevice<float>(1))
            using (var result_y_gpu = gpu.AllocateDevice<float>(1))
            {
                var vectors_x_ptr = vectors_x_gpu.Ptr;
                var vectors_y_ptr = vectors_y_gpu.Ptr;
                var result_x_ptr = result_x_gpu.Ptr;
                var result_y_ptr = result_y_gpu.Ptr;

                gpu.For(0, 1, no =>
                {
                    for (int i = 0; i < length; i++)
                    {
                        result_x_ptr[0] += vectors_x_ptr[i];
                        result_y_ptr[0] += vectors_y_ptr[i];
                    }
                });

                resultVector.X = Gpu.CopyToHost(result_x_gpu)[0];
                resultVector.Y = Gpu.CopyToHost(result_y_gpu)[0];
            }

            Console.WriteLine($"Сумма векторов = {{{resultVector.X}; {resultVector.Y}}}");

            Console.WriteLine("The end");
            Console.ReadKey();
        }

        private Vector2[] SetVectors()
        {
            int count = 5;
            var vectors = new Vector2[count];

            var rand = new Random();

            for (int i = 0; i < count; i++)
            {
                vectors[i] = new Vector2
                {
                    X = (float)rand.NextDouble() * 10,
                    Y = (float)rand.NextDouble() * 10
                };

                Console.WriteLine($"Вектор {i} - {{{vectors[i].X}; {vectors[i].Y}}}");
            }

            return vectors;
        }
    }
}
