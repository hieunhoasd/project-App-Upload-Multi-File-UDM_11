using System;

namespace FileUploadServer
{
    public static class ConsoleHelper
    {
        private static readonly object _lock = new object();

        public static void WriteLine(string message)
        {
            lock (_lock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            }
        }

        public static void WriteLine(string format, params object[] args)
        {
            lock (_lock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {string.Format(format, args)}");
            }
        }
    }
}