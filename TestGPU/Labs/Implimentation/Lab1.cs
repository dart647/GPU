using Alea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGPU.Labs.Interface;

namespace TestGPU.Labs
{
    class Lab1 : ILab
    {
        public void Execute()
        {
            Console.WriteLine("Lab 1 - Обращение к устройству");

            Gpu gpu = Gpu.Default;
            var device = gpu.Device;

            Console.WriteLine("GPU Name - " + device.Name);
            Console.WriteLine($"Computational Capabilities - {device.Arch.Major}.{device.Arch.Minor}");
            Console.WriteLine("Maximum global memory size - " + device.TotalMemory);
            Console.WriteLine("Maximum constant memory size - " + device.Properties.TotalConstantMemory);
            Console.WriteLine("Maximum shared memory size per block - " + device.Properties.SharedMemPerBlock);
            Console.WriteLine($"Maximum block dimensions - {device.Properties.MaxThreadsDim.x}x{device.Properties.MaxThreadsDim.y}x{device.Properties.MaxThreadsDim.z}");
            Console.WriteLine($"Maximum grid dimensions - {device.Properties.MaxGridSize.x}x{device.Properties.MaxGridSize.y}x{device.Properties.MaxGridSize.z}");
            Console.WriteLine("Warp size - " + device.Attributes.WarpSize);

            Console.ReadKey();
        }
    }
}
