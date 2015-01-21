using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ThreadDemo
{
    internal static class FalseSharing
    {
        [StructLayout(LayoutKind.Explicit)]
        private class Data
        {
            [FieldOffset(0)]
            public Int32 field1;

            [FieldOffset(64)]
            public Int32 field2;
        }

        private const Int32 iterations = 100000000;
        private static Int32 s_operations = 2;
        private static Int64 s_startTime;

        public static void Go()
        {
            Data data = new Data();
            s_startTime = Stopwatch.GetTimestamp();

            ThreadPool.QueueUserWorkItem(o => AccessData(data, 0));
            ThreadPool.QueueUserWorkItem(o => AccessData(data, 1));

            Console.ReadLine();
        }

        private static void AccessData(Data data, Int32 field)
        {
            for (Int32 x = 0; x < iterations; ++x)
            {
                if (field == 0)
                {
                    data.field1++;
                }
                else
                {
                    data.field2++;
                }
            }

            if (Interlocked.Decrement(ref s_operations) == 0)
            {
                Console.WriteLine("Total time: {0:N0}", Stopwatch.GetTimestamp() - s_startTime);
            }
        }
    }
}
