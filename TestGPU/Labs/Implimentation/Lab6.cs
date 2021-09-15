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
using System.CodeDom;

namespace TestGPU.Labs
{
    class Lab6 : ILab
    {
        public void Execute()
        {
            Console.WriteLine("Lab 6 - Стандартное произведение матриц");

            Gpu gpu = Gpu.Default;

            const int rows = 2;
            const int cols = rows;

            var matrixA = SetMatrix(rows, cols);
            Console.WriteLine($"Матрица A");
            ShowMatrix(matrixA, rows, cols);

            var matrixB = SetMatrix(rows, cols);
            Console.WriteLine($"Матрица B");
            ShowMatrix(matrixB, rows, cols);

            var resultMatrix = new double[rows, cols];

            using (var matrix_a_gpu = gpu.AllocateDevice(matrixA))
            using (var matrix_b_gpu = gpu.AllocateDevice(matrixB))
            using (var matrix_result_gpu = gpu.AllocateDevice(resultMatrix))
            {
                var matrix_a_ptr = matrix_a_gpu.Ptr;
                var matrix_b_ptr = matrix_b_gpu.Ptr;
                var matrix_result_ptr = matrix_result_gpu.Ptr;

                var pitch_a = matrix_a_gpu.PitchInElements.ToInt32();
                var pitch_b = matrix_b_gpu.PitchInElements.ToInt32();
                var pitch_result = matrix_result_gpu.PitchInElements.ToInt32();

                gpu.For(0, rows, r =>
                {
                    for (int c = 0; c < cols; c++)
                    {
                        var tmpResult = 0d;
                        for (int x = 0; x < rows; x++)
                        {
                            tmpResult += matrix_a_ptr[r * pitch_result + x] * matrix_b_ptr[x * pitch_result + c];
                        }
                        matrix_result_ptr[r * pitch_result + c] = tmpResult;
                    }
                });

                resultMatrix = Gpu.Copy2DToHost(matrix_result_gpu);
            }


            Console.WriteLine($"Результирующая матрица");
            ShowMatrix(resultMatrix, rows, cols);

            Console.WriteLine("The end");
            Console.ReadKey();
        }

        private double[,] SetMatrix(int rows, int cols)
        {
            var matrix = new double[rows, cols];

            var rand = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = rand.NextDouble() * 10;
                }
            }

            return matrix;
        }

        private void ShowMatrix(double[,] matrix, int rows, int cols)
        {
            Console.WriteLine();

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Console.Write($"{matrix[i, j].ToString("0.00")} ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}
