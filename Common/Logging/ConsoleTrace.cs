using System;

namespace Common.Logging
{
    public static class ConsoleTrace
    {
        public static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"{DateTime.Now.ToString()} - {message}");

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine($"{DateTime.Now.ToString()} - {message}");

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"{DateTime.Now.ToString()} - {message}");

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString()} - {message}");
        }
    }
}
