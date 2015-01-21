using System;
using System.Threading;

namespace ThreadDemo
{
    internal static class TimerDemo
    {
        private static Timer s_timer;

        public static void Go()
        {
            Console.WriteLine("Main thread: starting a timer");

            using (s_timer = new Timer(ComputeBoundOp, 5, 0, Timeout.Infinite))
            {
                Console.WriteLine("Main thread: doing other work here...");
                
                // Simulate other work
                Thread.Sleep(10000);
            }

            Console.ReadLine();
        }

        private static void ComputeBoundOp(object state)
        {
            Console.WriteLine("In ComputeBoundOp: state={0}", state);

            Thread.Sleep(1000);

            // Have the timer call this method again after 2 seconds.
            s_timer.Change(2000, Timeout.Infinite);
        }
    }
}
