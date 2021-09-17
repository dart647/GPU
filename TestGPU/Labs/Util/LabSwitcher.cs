using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGPU.Labs;
using TestGPU.Labs.Interface;

namespace TestGPU.Util
{
    class LabSwitcher
    {
        private ILab Lab { get; set; }
        private List<ILab> Labs { get; }

        public LabSwitcher()
        {
            Labs = new List<ILab>()
            {
                new Lab1(),
                new Lab2(),
                new Lab3(),
                new Lab4(),
                new Lab5(),
                new Lab6(),
                new Lab7(),
                new Lab8(),
                new Lab9()
            };
        }

        private bool SetLab(int labNumber)
        {
            var check = false;

            if(labNumber <= Labs.Count && labNumber > 0)
            {
                Lab = Labs[labNumber - 1];
                check = true;
            }

            return check;
        }

        public void ExecuteLab(int labNumber = 0)
        {
            if (SetLab(labNumber))
            {
                Console.WriteLine();

                try
                {
                    Lab.Execute();
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine($"Lab {labNumber} doesn't exists");
                Console.WriteLine();
            }
        }

    }
}
