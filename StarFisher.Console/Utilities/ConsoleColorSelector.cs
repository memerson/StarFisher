using System;

namespace StarFisher.Console.Utilities
{
    public static class ConsoleColorSelector
    {
        public static IDisposable SetConsoleForegroundColor(ConsoleColor consoleColor)
        {
            System.Console.ForegroundColor = consoleColor;
            return new Memento();
        }

        private class Memento : IDisposable
        {
            public void Dispose()
            {
                System.Console.ResetColor();
            }
        }
    }
}
