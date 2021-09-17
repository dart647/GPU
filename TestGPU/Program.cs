using Alea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGPU.Util;

namespace TestGPU
{
    class Program
    {
        static void Main(string[] args)
        {
            var isExit = false;
            var switcher = new LabSwitcher();

            do
            {
                Console.WriteLine("Choose lab");
                Console.WriteLine("1 - Lab 1 - Обращение к устройству");
                Console.WriteLine("2 - Lab 2 - Перевод цветного изображения в градации серого на CUDA");
                Console.WriteLine("3 - Lab 3 - Размытие изображения на CUDA");
                Console.WriteLine("4 - Lab 4 - Сложение векторов на CUDA");
                Console.WriteLine("5 - Lab 5 - Блочное перемножение матриц на CUDA");
                Console.WriteLine("6 - Lab 6 - Стандартное произведение матриц");
                Console.WriteLine("7 - Lab 7 - Гистограмма текста");
                Console.WriteLine("8 - Lab 8 - Гистограмма");
                Console.WriteLine("9 - Lab 9 - Разностная схема");
                Console.WriteLine("0 - Exit");

                var key = Console.ReadLine();
                if(Int16.TryParse(key, out short num))
                {
                    if(!(isExit = num == 0))
                        switcher.ExecuteLab(num);
                }
                else
                {
                    Console.WriteLine("Wrong Key");
                    Console.WriteLine();
                }
            } 
            while (!isExit);
        }
    }
}
