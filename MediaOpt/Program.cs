using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOpt
{
    internal class Program
    {
        private static readonly ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();
        private static int _counter;

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            var totalStopwatch = new Stopwatch();
            totalStopwatch.Start();
            ProcessDir(args[0]);
            Console.WriteLine("Count: {0}", Queue.Count);
            Parallel.ForEach(Queue, ProcessFile);
            totalStopwatch.Stop();
            Console.WriteLine("Processed in: {0}s", totalStopwatch.ElapsedMilliseconds/1000);
        }

        private static void ProcessDir(string path)
        {
            var files = Directory.EnumerateFiles(path);
            Parallel.ForEach(files, s =>
            {
                if (s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    Queue.Enqueue(s);
                }
            });
            var dirs = Directory.EnumerateDirectories(path);
            Parallel.ForEach(dirs, ProcessDir);
        }

        private static void ProcessFile(string path)
        {
            var fileStopwatch = new Stopwatch();
            fileStopwatch.Start();
            var target = path.Remove(path.LastIndexOf(".", StringComparison.Ordinal)) + ".webp";
            IConverter converter = new WebPConverter();
            converter.Convert(path, target);
            File.Delete(path);
            fileStopwatch.Stop();
            Interlocked.Increment(ref _counter);
            Console.WriteLine("Processed {2}: {0} in {1}s", target, fileStopwatch.ElapsedMilliseconds/1000, _counter);
        }
    }
}