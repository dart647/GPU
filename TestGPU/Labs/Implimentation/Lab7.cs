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
    class Lab7 : ILab
    {
        public void Execute()
        {
            Console.WriteLine("Lab 7 - Гистограмма текста");

            var text = SetText();
            var gistoIntervals = SetGistoIntervals();
            var asciiText = Encoding.ASCII.GetBytes(text);

            ShowText(asciiText);

            Parallel.For(0, gistoIntervals.Length, new Action<int>(gi =>
            {
                var symbolCount = asciiText.Length;

                var gistoInterval = gistoIntervals[gi];
                var from = gistoInterval.from;
                var to = gistoInterval.to;

                for (int c = 0; c < symbolCount; c++)
                {
                    gistoInterval.count += asciiText[c] >= from && asciiText[c] <= to ? 1 : 0;
                }
                gistoIntervals[gi] = gistoInterval;
            }));

            ShowGisto(gistoIntervals);

            Console.WriteLine("The end");
            Console.ReadKey();
        }

        private string SetText()
        {
            var text = "";
            var rand = new Random(DateTime.Now.Millisecond);
            var maxTextLength = rand.Next(10, 20);

            for (int i = 0; i < maxTextLength; i++)
            {
                rand = new Random(DateTime.Now.Millisecond + i);
                text += Convert.ToChar(rand.Next(128));
            }

            return text;
        }

        private Interval[] SetGistoIntervals()
        {
            var rand = new Random(DateTime.Now.Millisecond);

            var intervalCount = rand.Next(2, 10);
            var intervalLength = Math.Ceiling(128d / intervalCount);

            var intervals = new Interval[intervalCount];

            for (int i = 0; i < intervalCount; i++)
            {
                var to = intervalLength * i + intervalLength;
                intervals[i] = new Interval()
                {
                    from = (byte)(intervalLength * i),
                    to = to > 127 ? (byte)127 : (byte)(to),
                    count = 0
                };
            }

            return intervals;
        }

        private void ShowGisto(Interval[] gisto)
        {
            Console.WriteLine();

            var from = Encoding.ASCII.GetChars(gisto.Select(g => g.from).ToArray());
            var to = Encoding.ASCII.GetChars(gisto.Select(g => g.to).ToArray());

            for (int i = 0; i < gisto.Length; i++)
            {
                Console.WriteLine($"{from[i]}-{to[i]}: {gisto[i].count}");
            }

            Console.WriteLine();
        }

        private void ShowText(byte[] text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Console.Write(Convert.ToChar(text[i]));
            }
            Console.WriteLine();
        }

        private struct Interval
        {
            public byte from { get; set; }
            public byte to { get; set; }
            public int count { get; set; }
        }
    }
}
