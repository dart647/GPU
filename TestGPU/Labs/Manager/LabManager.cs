using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGPU.Labs;
using TestGPU.Labs.Interface;

namespace TestGPU
{
    class LabManager
    {
        private ILab Lab { get; set; }
        private List<ILab> Labs { get; }

        public LabManager()
        {
            Lab = new Lab1();
            Labs = new List<ILab>()
            {
                new Lab1(),
                new Lab2(),
                new Lab3(),
                new Lab4(),
                new Lab5()
            };
        }

        private void SetLab(int labNumber)
        {
            Lab = Labs[labNumber - 1];
        }

        public void ExecuteLab(int labNumber = 0)
        {
            SetLab(labNumber);

            Console.WriteLine();

            try
            {
                Lab.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }

    }
}
