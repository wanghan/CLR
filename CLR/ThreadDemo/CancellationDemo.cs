using System;
using System.Threading;

namespace ThreadDemo
{
    internal static class CancellationDemo
    {
        public static void Go()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            ThreadPool.QueueUserWorkItem(o => Count(cts.Token, 10));

            Console.WriteLine("Press <enter> to cancel the operation");

            Console.ReadLine();
            cts.Cancel();
        }

        private static void Count(CancellationToken token, Int32 countTo)
        {
            for (Int32 count = 0; count < countTo; ++count)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Count is cancelled.");
                    break;
                }

                Console.WriteLine(count);
                Thread.Sleep(200);
            }

            Console.WriteLine("Count is done.");
        }
    }
}
