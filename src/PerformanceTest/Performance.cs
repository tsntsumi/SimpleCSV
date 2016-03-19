//
// Program.cs
//
// Author:
//       tsntsumi <tsntsumi@tsntsumi.com>
//
// Copyright (c) 2016 tsntsumi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SimpleCSV;

namespace PerformanceTest
{
    class Performance
    {
        private readonly string file;

        private List<AbstractParser> parsers = new List<AbstractParser>() {
            new SimpleCSVParser(),
            new ReadLineAndSplitParser(),
            new CsvHelperParser(),
        };

        Performance(string file)
        {
            this.file = file;
        }

        private long Run(AbstractParser parser)
        {
            var sw = new Stopwatch();
            sw.Start();
            parser.ProcessLines(file);
            sw.Stop();

            long time = sw.ElapsedMilliseconds;
            Console.WriteLine("took {0} ms to read {1} lines.", time, parser.LineCount);
            parser.ResetLineCount();
            Environment.SetEnvironmentVariable("BLACKHOLE", parser.Blackhole.ToString());
            return time;
        }

        public void Execute(int loops)
        {
            var stats = new Dictionary<string, long[]>();

            foreach (var parser in parsers)
            {
                long[] times = new long[loops];
                for (int i = 0; i < times.Length; i++)
                {
                    times[i] = -1;
                }
                stats[parser.Name] = times;
            }

            for (int i = 0; i < loops; i++)
            {
                foreach (var parser in parsers)
                {
                    try
                    {
                        Console.Write("Loop {0} - execute {1}... ", i + 1, parser.Name);
                        long time = Run(parser);
                        stats[parser.Name][i] = time;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Parser {0} threw exception {1}", parser.Name, ex.Message);
                    }
                    GC.Collect();
                    Thread.Sleep(500);
                }
            }
            PrintResults(loops, stats);
        }

        private void PrintResults(int loops, IDictionary<string, long[]> stats)
        {
            Console.WriteLine("\n==========\n AVERAGES \n==========\n");

            var averages = OrderByAverageTime(stats);
            long bestTime = 0;
            foreach (var average in averages)
            {
                long time = average.Key;
                string parser = average.Value;
                Console.Write("| {0} \t | {1} ms ", parser, time);
                if (time == -1)
                {
                    Console.WriteLine("Could not execute");
                    continue;
                }
                if (bestTime != 0)
                {
                    long increasePrecentage = time * 100 / bestTime - 100;
                    Console.Write(" \t | {0}% ", increasePrecentage);
                }
                else
                {
                    bestTime = time;
                    Console.Write(" \t | Best time! ");
                }
                Console.WriteLine();
            }
        }

        private SortedDictionary<long, string> OrderByAverageTime(IDictionary<string, long[]> stats)
        {
            var averages = new SortedDictionary<long, string>();

            foreach (var parserTimes in stats)
            {
                long[] times = parserTimes.Value;
                long average = 0;
                //we are discarding the first recorded time here to take into account JIT optimizations
                for (int i = 1; i < times.Length; i++)
                {
                    average += times[i];
                }
                average /= times.Length - 1;
                averages[average] = parserTimes.Key;
            }

            return averages;
        }

        public static void Main(string[] args)
        {
            int loops = 6;

            new Performance("../../worldcitiespop.txt").Execute(loops);
        }
    }
}
