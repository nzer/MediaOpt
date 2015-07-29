using System.Diagnostics;

namespace MediaOpt
{
    public class WebPConverter : IConverter
    {
        public void Convert(string source, string target)
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