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
    class Lab5 : ILab
    {
        public void Execute()
        {
            Console.WriteLine("Lab 5 - Блочное перемножение матриц на CUDA");

            Gpu gpu = Gpu.Default;

            const int rows = 2;
            const int cols = rows;
            int blocks = 2;

            var matrixA = SetBlockMatrix(rows, cols, blocks);
            Console.WriteLine($"Матрица A");
            ShowMatrix(matrixA, rows, cols, blocks);

            var matrixB = SetBlockMatrix(rows, cols, blocks);
            Console.WriteLine($"Матрица B");
            ShowMatrix(matrixB, rows, cols, blocks);

            var resultMatrix = new Matrix[blocks, blocks];

            for (int i = 0; i < blocks; i++)
            {
                for (int j = 0; j < blocks; j++)
                {
                    var tmpMatrix = new Matrix[blocks];
                    for (int y = 0; y < blocks; y++)
                    {
                        using (var matrix_a_gpu = gpu.AllocateDevice(matrixA[i, y].Block))
                        using (var matrix_b_gpu = gpu.AllocateDevice(matrixB[y, j].Block))
                        using (var matrix_result_gpu = gpu.AllocateDevice<double>(rows, cols))
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

                            tmpMatrix[y].Block = Gpu.Copy2DToHost(matrix_result_gpu);
                        }
                    }

                    for (int y = 0; y < blocks - 1; y++)
                    {
                        using (var matrix_a_gpu = gpu.AllocateDevice(tmpMatrix[y].Block))
                        using (var matrix_b_gpu = gpu.AllocateDevice(tmpMatrix[y + 1].Block))
                        using (var matrix_result_gpu = gpu.AllocateDevice<double>(rows, cols))
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
                                    tmpResult += matrix_a_ptr[r * pitch_result + c] + matrix_b_ptr[r * pitch_result + c];
                                    matrix_result_ptr[r * pitch_result + c] = tmpResult;
                                }
                            });

                            tmpMatrix[y + 1].Block = Gpu.Copy2DToHost(matrix_result_gpu);
                        }
                    }

                    resultMatrix[i, j].Block = tmpMatrix[blocks - 1].Block;
                }
            }


            Console.WriteLine($"Результирующая матрица");
            ShowMatrix(resultMatrix, rows, cols, blocks);

            Console.WriteLine("The end");
            Console.ReadKey();
        }

        private Matrix[,] SetBlockMatrix(int rows, int cols, int blocks)
        {
            var matrix = new Matrix[blocks, blocks];

            var rand = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < blocks; i++)
            {
                for (int j = 0; j < blocks; j++)
                {
                    matrix[i, j] = new Matrix();
                    matrix[i, j].Block = new double[rows, cols];
                    for (int r = 0; r < rows; r++)
                    {
                        for (int c = 0; c < cols; c++)
                        {
                            matrix[i, j].Block[r, c] = rand.NextDouble() * 10;
                        }
                    }
                }
            }

            return matrix;
        }

        private void ShowMatrix(Matrix[,] matrix, int rows, int cols, int blocks)
        {
            Console.WriteLine();
            for (int b2 = 0; b2 < blocks; b2++)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int b1 = 0; b1 < blocks; b1++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            Console.Write(matrix[b2, b1].Block[i, j].ToString("0.00") + "\t");
                        }
                        Console.Write("\t|\t");
                    }
                    Console.WriteLine();
                }
                for (int i = 0; i < cols * blocks * 2; i++)
                {
                    Console.Write("_____\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private struct Matrix
        {
            public double[,] Block { get; set; }
        }
    }
}
