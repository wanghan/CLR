using System;
using System.Threading;

namespace ThreadDemo
{
    internal static class CancellationLinkDemo
    {
        public static void Go()
        {
            // Create a CancellationTokenSource
            var cts1 = new CancellationTokenSource();
            cts1.Token.Register(() => Console.WriteLine("cts1 canceled"));

            // Create another CancellationTokenSource
            var cts2 = new CancellationTokenSource();
            cts2.Token.Register(() => Console.WriteLine("cts2 canceled"));

            // Create a new CancellationTokenSource that is canceled when cts1 or cts2 is canceled.
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token);
            linkedCts.Token.Register(() => Console.WriteLine("linked cts canceld"));

            ThreadPool.QueueUserWorkItem(o => CancellationDemo.Count(linkedCts.Token, 10));

            Console.WriteLine("Press <enter> to cancel the operation");

            // Cancel one of the CancellationToken
            Console.ReadLine();
            cts2.Cancel();
            Console.ReadLine();

            Console.WriteLine("cts1 canceled={0}, cts2 canceled={1}, linkedCts canceled={2},"
                , cts1.IsCancellationRequested
                , cts2.IsCancellationRequested
                , linkedCts.IsCancellationRequested);
        }
    }
}
