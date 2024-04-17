using System;
using System.Diagnostics;

namespace Qtwols
{
    class Instrumentation
    {
        readonly Stopwatch jobWatch, stepWatch;

        public Instrumentation()
        {
            jobWatch = new Stopwatch();
            jobWatch.Start();
            stepWatch = new Stopwatch();
            stepWatch.Start();
        }

        public void StepComplete(string step)
        {
            stepWatch.Stop();
            Console.WriteLine($"{step} completed in {stepWatch.ElapsedMilliseconds} ms");
            stepWatch.Reset();
            stepWatch.Start();
        }

        public void JobComplete()
        {
            jobWatch.Stop();
            stepWatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"Total job time was {jobWatch.ElapsedMilliseconds} ms");
        }
    }
}
