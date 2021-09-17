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
    class Lab8 : ILab
    {
        public void Execute()
        {
            Console.WriteLine("Lab 8 - Гистограмма");

            var gistoIntervals = SetGistoIntervals();
            var intArray = SetArray();

            Console.WriteLine($"Array count - {intArray.Length}");

            Parallel.For(0, gistoIntervals.Length, new Action<int>(gi =>
            {
                var intCount = intArray.Length;

                var gistoInterval = gistoIntervals[gi];
                var from = gistoInterval.from;
                var to = gistoInterval.to;

                for (int c = 0; c < intCount; c++)
                {
                    if (gistoInterval.count < 127)
                        gistoInterval.count += intArray[c] >= from && intArray[c] <= to ? 1 : 0;
                }
                gistoIntervals[gi] = gistoInterval;
            }));

            ShowGisto(gistoIntervals);

            Console.WriteLine("The end");
            Console.ReadKey();
        }

        private int[] SetArray()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var maxArrayLength = rand.Next(1, Convert.ToInt32(Math.Pow(2, 10)));

            var array = new int[maxArrayLength];

            for (int i = 0; i < maxArrayLength; i++)
            {
                rand = new Random(DateTime.Now.Millisecond + i);
                array[i] = rand.Next(4096);
            }

            return array;
        }

        private Interval[] SetGistoIntervals()
        {
            var rand = new Random(DateTime.Now.Millisecond);

            var intervalCount = rand.Next(2, 10);
            var intervalLength = Convert.ToInt32(Math.Ceiling(4096d / intervalCount));

            var intervals = new Interval[intervalCount];

            for (int i = 0; i < intervalCount; i++)
            {
                var to = intervalLength * i + intervalLength;
                intervals[i] = new Interval()
                {
                    from = intervalLength * i,
                    to = to > 4096 ? 4096 : to,
                    count = 0
                };
            }

            return intervals;
        }

        private void ShowGisto(Interval[] gisto)
        {
            Console.WriteLine();

            var from = gisto.Select(g => g.from).ToArray();
            var to = gisto.Select(g => g.to).ToArray();

            for (int i = 0; i < gisto.Length; i++)
            {
                Console.WriteLine($"{from[i]}-{to[i]}: {gisto[i].count}");
            }

            Console.WriteLine();
        }

        private struct Interval
        {
            public int from { get; set; }
            public int to { get; set; }
            public int count { get; set; }
        }
    }
}
