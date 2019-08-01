using System;

namespace StarFisher.Console.Utilities
{
    public static class ConsoleColorSelector
    {
        public static IDisposable SetConsoleForegroundColor(ConsoleColor consoleColor)
        {
            var currentForegroundColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = consoleColor;
            return new ForegroundMemento(currentForegroundColor);
        }

        public static IDisposable SetConsoleBackgroundColor(ConsoleColor consoleColor)
        {
            var currentBackgroundColor = System.Console.BackgroundColor;
            System.Console.BackgroundColor = consoleColor;
            return new BackgroundMemento(currentBackgroundColor);
        }

        private class ForegroundMemento : IDisposable
        {
            private readonly ConsoleColor _previousForegroundColor;

            public ForegroundMemento(ConsoleColor previousForegroundColor)
            {
                _previousForegroundColor = previousForegroundColor;
            }

            public void Dispose()
            {
                System.Console.ForegroundColor = _previousForegroundColor;
            }
        }

        private class BackgroundMemento : IDisposable
        {
            private readonly ConsoleColor _previousBackgroundColor;

            public BackgroundMemento(ConsoleColor previousBackgroundColor)
            {
                _previousBackgroundColor = previousBackgroundColor;
            }

            public void Dispose()
            {
                System.Console.BackgroundColor = _previousBackgroundColor;
            }
        }
    }
}