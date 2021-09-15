using Alea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGPU
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose lab");
            Console.WriteLine("1 - Lab 1 - Обращение к устройству");
            Console.WriteLine("2 - Lab 2 - Перевод цветного изображения в градации серого на CUDA");
            Console.WriteLine("3 - Lab 3 - Размытие изображения на CUDA");
            Console.WriteLine("4 - Lab 4 - Сложение векторов на CUDA");

            var labNumber = Int16.Parse(Console.ReadLine());

            var manager = new LabManager();

            manager.ExecuteLab(labNumber);
        }
    }
}
