using System;
using StarFisher.Console.Utilities;

namespace StarFisher.Console.Menu.Common
{
    public class MenuItem
    {
        private readonly RunCommand _runCommand;

        public delegate bool RunCommand(out Exception exception);

        public MenuItem(string title, RunCommand runCommand)
        {
            _runCommand = runCommand;
            if(string.IsNullOrWhiteSpace(title))
                throw new ArgumentException(nameof(title));

            Title = title;
            _runCommand = runCommand ?? throw new ArgumentNullException(nameof(runCommand));
        }

        public string Title { get; }

        public void Run()
        {
            if(!_runCommand(out Exception exception))
                PrintException(exception);
        }

        private static void PrintException(Exception exception)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.Red))
            {
                System.Console.WriteLine();
                System.Console.WriteLine(exception?.ToString() ?? "UNKNOWN ERROR" );
                System.Console.WriteLine();
            }
        }
    }
}
