using System;
using System.Threading;

namespace ThreadDemo
{
    internal static class CancellationDemo
    {
        public static void Go()
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();

                cts.Token.Register(AfterCancelCallbackWithException);
                cts.Token.Register(AfterCancelCallbackWithException);

                ThreadPool.QueueUserWorkItem(o => Count(cts.Token, 10));

                Console.WriteLine("Press <enter> to cancel the operation");

                Console.ReadLine();

                // True means the first callback method that throws an unhandled exception stops the other callbacks 
                cts.Cancel(true);
                Console.ReadLine();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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

        private static void AfterCancelCallbackWithException()
        {
            Console.WriteLine("After cancel callback with Exception is invoking.");
            throw new Exception(string.Format("Exception is thrown at {0}", DateTime.Now));
        }
    }
}
