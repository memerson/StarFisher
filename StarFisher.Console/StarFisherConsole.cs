using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Utilities;

namespace StarFisher.Console
{
    public interface IStarFisherConsole
    {
        void SetCurrentCommandTitle(string commandTitle);
        void ClearCurrentCommandTitle();
        void SetSuccessMessage(string successMessage);
        void SetErrorMessage(string errorMessage);
        void PrintSplash();
        void ResetConsole(bool printCommandTitle = true);
        void Write(string text);
        void WriteRed(string text);
        void WriteLine(string text, params string[] redTokens);
        void WriteLine();
        void WriteLineRed(string text);
        void WriteLineBlue(string text, params string[] redTokens);
        void WriteLineYellow(string text);
        void WriteInputPrompt();
        void WaitForKeyPress();
        string ReadLine();
        void ClearLastLine();
        void PrintException(Exception exception);
    }

    public class StarFisherConsole : IStarFisherConsole
    {
        private string _currentCommandTitle;
        private string _successMessage;
        private string _errorMessage;

        static StarFisherConsole()
        {
            Instance = new StarFisherConsole();
            System.Console.SetWindowSize(
                System.Console.LargestWindowWidth * 2 / 3,
                System.Console.LargestWindowHeight - 10);

            System.Console.BackgroundColor = ConsoleColor.White;
            System.Console.ForegroundColor = ConsoleColor.Black;
        }

        private StarFisherConsole() { }

        public static StarFisherConsole Instance { get; }

        public void SetCurrentCommandTitle(string commandTitle)
        {
            if (string.IsNullOrWhiteSpace(commandTitle))
                throw new ArgumentException(nameof(commandTitle));

            _currentCommandTitle = commandTitle;
        }

        public void ClearCurrentCommandTitle()
        {
            _currentCommandTitle = null;
        }

        public void SetSuccessMessage(string successMessage)
        {
            if (string.IsNullOrWhiteSpace(successMessage))
                throw new ArgumentException(nameof(successMessage));

            _successMessage = successMessage;
        }

        public void SetErrorMessage(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException(nameof(errorMessage));

            _errorMessage = errorMessage;
        }

        public void PrintSplash()
        {
            System.Console.WriteLine();
            System.Console.WriteLine(@"        .");
            System.Console.WriteLine(@"       ,O,");
            System.Console.WriteLine(@"      ,OOO,");
            System.Console.WriteLine(@"'oooooOOOOOooooo'     _________ __              ___________.__       .__");
            System.Console.WriteLine(@"  `OOOOOOOOOOO`      /   _____//  |______ ______\_   _____/|__| _____|  |__   ___________  ");
            System.Console.WriteLine(@"    `OOOOOOO`        \_____  \\   __\__  \\_  __ \    __)  |  |/  ___/  |  \_/ __ \_  __ \ ");
            System.Console.WriteLine(@"    OOOO'OOOO        /        \|  |  / __ \|  | \/     \   |  |\___ \|   Y  \  ___/|  | \/ ");
            System.Console.WriteLine(@"   OOO'   'OOO      /_______  /|__| (____  /__|  \___  /   |__/____  >___|  /\___  >__|    ");
            System.Console.WriteLine(@"  O'         'O             \/           \/          \/            \/     \/     \/        ");
            System.Console.WriteLine(@"                                                                         Nashville Edition ");
            System.Console.WriteLine(@"                                                                         By Matt Emerson   ");
            System.Console.WriteLine();
            System.Console.WriteLine();
        }

        public void ResetConsole(bool printCommandTitle = true)
        {
            System.Console.Clear();
            PrintSplash();

            if (printCommandTitle)
                PrintCommandTitle();

            PrintSuccessMessage();
            PrintErrorMessage();
        }

        public void Write(string text) => System.Console.Write(text);

        public void WriteRed(string text)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.Red))
                Write(text);
        }

        public void WriteLine(string text, params string[] redTokens)
        {
            if (string.IsNullOrEmpty(text))
            {
                WriteLine();
                return;
            }

            var redTokensLookup = new HashSet<string>(redTokens ?? Enumerable.Empty<string>(),
                StringComparer.InvariantCultureIgnoreCase);

            var tokens = text.Split(' ');
            var maxLineLength = System.Console.BufferWidth - 5;
            var charactersWritten = 0;

            foreach (var token in tokens)
            {
                if (charactersWritten + token.Length > maxLineLength)
                {
                    WriteLine();
                    charactersWritten = 0;
                }
                else if (charactersWritten > 0)
                {
                    Write(" ");
                }

                if (redTokensLookup.Contains(token))
                    WriteRed(token);
                else
                    Write(token);

                charactersWritten += token.Length + 1;
            }

            WriteLine();
        }

        public void WriteLine() => System.Console.WriteLine();

        public void WriteLineRed(string text)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.Red))
                WriteLine(text);
        }

        public void WriteLineBlue(string text, params string[] redTokens)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.DarkBlue))
                WriteLine(text, redTokens);
        }

        public void WriteLineYellow(string text)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.DarkYellow))
                WriteLine(text);
        }

        public void WriteInputPrompt() => Write(@"> ");

        public void WaitForKeyPress() => System.Console.ReadKey();

        public string ReadLine() => System.Console.ReadLine()?.Trim();

        public void ClearLastLine()
        {
            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
            System.Console.Write(new string(' ', System.Console.BufferWidth));
            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
        }

        public void PrintException(Exception exception)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.Red))
            {
                WriteLine();
                WriteLine(@"The operation has failed.");
                WriteLine();
                WriteLine(exception?.ToString() ?? "UNKNOWN ERROR");
            }

            WriteLine(@"Press any key to continue.");
            WriteInputPrompt();
            WaitForKeyPress();
            ResetConsole();
        }

        private void PrintCommandTitle()
        {
            PrintText(_currentCommandTitle, ConsoleColor.DarkGray);
        }

        private void PrintSuccessMessage()
        {
            PrintText(_successMessage, ConsoleColor.DarkGreen);
            _successMessage = null;
        }

        private void PrintErrorMessage()
        {
            PrintText(_errorMessage, ConsoleColor.Red);
            _errorMessage = null;
        }

        private void PrintText(string text, ConsoleColor color)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            using (ConsoleColorSelector.SetConsoleForegroundColor(color))
            {
                WriteLine();
                WriteLine(text);
            }
        }
    }
}
