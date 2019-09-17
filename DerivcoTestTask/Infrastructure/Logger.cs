using System;
using System.IO;
using System.Threading.Tasks;

namespace DerivcoTestTask.Infrastructure
{
    public static class Logger
    {
        // I used this approach of setting path with assumption that
        // application won't work non-stop
        private static string _path = $"{Path.GetTempPath()}\\DerivcoTestTask\\Logs\\{DateTime.Now.ToShortDateString()}.txt";

        static Logger()
        {
            var directory = Path.GetDirectoryName(_path);
            Directory.CreateDirectory(directory);
        }

        public static void Write(string message)
        {
            File.AppendAllText(_path, message + Environment.NewLine);
        }

        public static async Task WriteAsync(string message)
        {
            await File.AppendAllTextAsync(_path, message + Environment.NewLine);
        }
    }
}
