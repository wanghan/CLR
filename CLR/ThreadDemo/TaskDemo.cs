using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo
{
    internal class TaskDemo
    {
        public static void TaskFactoryDemo()
        {
            Task parent = new Task(() =>
            {
                var cts = new CancellationTokenSource();
                var tf = new TaskFactory<Int32>(cts.Token,
                    TaskCreationOptions.AttachedToParent,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);

                // This tasks creates and starts 3 child tasks
                var childTasks = new[]{
                    tf.StartNew(() => Sum(cts.Token, 10000)),
                    tf.StartNew(() => Sum(cts.Token, 20000)),
                    tf.StartNew(() => Sum(cts.Token, 30000)),
                };

                // If any of the child task throw, cancel the rest of them
                foreach (Task<Int32> task in childTasks)
                {
                    task.ContinueWith(t => cts.Cancel(), TaskContinuationOptions.OnlyOnFaulted);
                }

                // When all children are done, get the max value from from each non-faulting/canceled tasks.
                // Then pass the max value to another task with displays the max result.
                tf.ContinueWhenAll(childTasks,
                    completedTask => completedTask.Where(t => !t.IsFaulted && !t.IsCanceled).Max(t => t.Result),
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default)
                    .ContinueWith(t => Console.WriteLine("Max result: " + t.Result), TaskContinuationOptions.ExecuteSynchronously);
            });

            // When the children are done. show any unhandled exceptions
            parent.ContinueWith(p =>
            {
                StringBuilder sb = new StringBuilder("The following exception occurred: " + Environment.NewLine);

                foreach (var e in p.Exception.Flatten().InnerExceptions)
                {
                    sb.AppendLine("    " + e.GetType().ToString());
                }

                Console.WriteLine(sb.ToString());
            }, TaskContinuationOptions.OnlyOnFaulted);

            parent.Start();

            Console.WriteLine("Wait here.");
            Console.ReadLine();
        }

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
