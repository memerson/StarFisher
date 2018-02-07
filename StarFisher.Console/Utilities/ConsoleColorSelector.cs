using System;

namespace StarFisher.Console.Utilities
{
    public static class ConsoleColorSelector
    {
        public static IDisposable SetConsoleForegroundColor(ConsoleColor consoleColor)
        {
            var currentForegroundColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = consoleColor;
            return new Memento(currentForegroundColor);
        }

        private class Memento : IDisposable
        {
            private readonly ConsoleColor _previousForegroundColor;

            public Memento(ConsoleColor previousForegroundColor)
            {
                _previousForegroundColor = previousForegroundColor;
            }

            public void Dispose()
            {
                System.Console.ForegroundColor = _previousForegroundColor;
            }
        }
    }
}
