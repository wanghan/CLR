using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace ThreadDemo
{
    class ThreadPoolSample
    {
        static void Main(string[] args)
        {
            ParallelDemo.ObsoleteMethods(Assembly.LoadFile(@"D:\LabExplorer\MSTSCLib.dll"));
        }

        private static void ThreadPoolDemo()
        {
            Console.WriteLine("Main thread: starting a dedicated thread to do an async opertaion");

            ThreadPool.QueueUserWorkItem(ComputeBoundOp, 5);

            Console.WriteLine("Main thread: Doing other work here...");

            Thread.Sleep(10000);

            Console.WriteLine("Hit <Enter> to end this program.");
            Console.ReadLine();
        }

        private static void ComputeBoundOp(object state)
        {
            Console.WriteLine("In compute-BoundOp: state={0}", state);

            Thread.Sleep(1000);
        }

        private static void ExecutionContextsDemo()
        {
            CallContext.LogicalSetData("Name", "Han");

            ThreadPool.QueueUserWorkItem(state => Console.WriteLine("Name={0}", CallContext.LogicalGetData("Name")));

            ExecutionContext.SuppressFlow();

            ThreadPool.QueueUserWorkItem(state => Console.WriteLine("Name={0}", CallContext.LogicalGetData("Name")));

            ExecutionContext.RestoreFlow();

            Thread.Sleep(10000);

            Console.WriteLine("Hit <Enter> to end this program.");
            Console.ReadLine();
        }
    }
}
