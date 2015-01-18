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

            Task<Int32> task = new Task<Int32>(() => Sum(cts.Token, 100000), cts.Token);
            task.Start();

            // cts.Cancel();

            task.ContinueWith(t => Console.WriteLine("The sum is " + t.Result),
                TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => Console.WriteLine("Exception threw: " + t.Exception),
                TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => Console.WriteLine("Task cancled"),
                TaskContinuationOptions.OnlyOnCanceled);

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
