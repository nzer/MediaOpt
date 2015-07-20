using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MediaOpt
{
    internal class Program
    {
        private static readonly ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();

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

        /// <summary>
        ///     Returns all jpeg files in given directory. Recursive
        /// </summary>
        /// <param name="path"></param>
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
            RunWebP(path, target);
            File.Delete(path);
            fileStopwatch.Stop();
            Console.WriteLine("Processed: {0} in {1}s", target, fileStopwatch.ElapsedMilliseconds/1000);
        }

        private static void RunWebP(string source, string target)
        {
            var commandline =
                string.Format(
                    @"-preset photo -m 6 -q 85 -mt -metadata all -sns 85 ""{0}"" -o ""{1}""", source, target);

            var p = new Process();
            p.StartInfo.FileName = @"redist\cwebp.exe";
            p.StartInfo.Arguments = commandline;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.WaitForExit();
        }
    }
}