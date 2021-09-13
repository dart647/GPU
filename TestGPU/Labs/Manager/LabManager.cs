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

        public LabManager()
        {
            Lab = new Lab1();
        }

        private void SetLab(int labNumber)
        {
            switch (labNumber)
            {
                case 2:
                    {
                        Lab = new Lab2();
                        break;
                    }
                case 3:
                    {
                        Lab = new Lab3();
                        break;
                    }
                    //case 4:
                    //    {
                    //        Lab = new Lab1();
                    //        break;
                    //    }
                    //case 5:
                    //    {
                    //        Lab = new Lab1();
                    //        break;
                    //    }
                    //case 6:
                    //    {
                    //        Lab = new Lab1();
                    //        break;
                    //    }
                    //case 7:
                    //    {
                    //        Lab = new Lab1();
                    //        break;
                    //    }
                    //case 8:
                    //    {
                    //        Lab = new Lab1();
                    //        break;
                    //    }
                    //case 9:
                    //    {
                    //        Lab = new Lab1();
                    //        break;
                    //    }
                    //case 10:
                    //    {
                    //        Lab = new Lab1();
                    //        break;
                    //    }
            }
        }

        public void ExecuteLab(int labNumber = 1)
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
