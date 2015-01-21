using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo
{
    internal static class ParallelDemo
    {
        public static Int64 DirecrtoryBytes(string path, string searchPattern, SearchOption searchOption)
        {
            var files = Directory.EnumerateFiles(path, searchPattern, searchOption);
            Int64 masterLength = 0;

            ParallelLoopResult result = Parallel.ForEach<string, Int64>(
                files,

                () =>
                {
                    return 0;
                },

                (file, loopState, index, taskLocalTotal) =>
                {
                    Int64 fileLength = 0;
                    try
                    {
                        using (FileStream fs = File.OpenRead(file))
                        {
                            fileLength = fs.Length;
                        }
                    }
                    catch (IOException) { }
                    return taskLocalTotal + fileLength;
                },

                taskLocalTotal =>
                {
                    // MasterLength should be updated in a thread-safe way.
                    Interlocked.Add(ref masterLength, taskLocalTotal);
                }
                );

            return masterLength;
        }
    }
}
