using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo
{
    internal class TaskDemo
    {
        public static void TaskCancellationDemo()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task<Int32> task = new Task<Int32>(() => Sum(cts.Token, 10000), cts.Token);
            task.Start();

            //cts.Cancel();

            try
            {
                // If the task got canceled, result will throw an AggregateException
                // task.Result will block the current thread.
                // Console.WriteLine("The sum is " + task.Result);

                task.ContinueWith(t => Console.WriteLine("The sum is " + t.Result));
            }
            catch (AggregateException ae)
            {
                // Consider any OperationCanceledException objects as handled.
                // Any other exception cause a new AggregateException containing only the unhandled exceptions to be thrown.
                ae.Handle(e => e is OperationCanceledException);

                Console.WriteLine("Sum was canceled.");
            }

            Console.WriteLine("Wait here.");
            Console.ReadLine();
        }

        private static Int32 Sum(CancellationToken token, Int32 n)
        {
            Int32 sum = 0;
            for (; n > 0; n--)
            {
                token.ThrowIfCancellationRequested();

                // if n is large, this will throw System.OverflowException
                checked { sum += n; }
            }

            return sum;
        }
    }
}
