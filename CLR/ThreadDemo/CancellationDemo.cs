﻿using System;
using System.Threading;

namespace ThreadDemo
{
    internal static class CancellationDemo
    {
        public static void Go()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            cts.Token.Register(AfterCancelCallback);

            ThreadPool.QueueUserWorkItem(o => Count(cts.Token, 10));

            Console.WriteLine("Press <enter> to cancel the operation");

            Console.ReadLine();
            cts.Cancel();
            Console.ReadLine();
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

        private static void AfterCancelCallback()
        {
            Console.WriteLine("After cancel callback is invoking.");
        }
    }
}