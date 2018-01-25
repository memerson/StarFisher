
using System;

namespace StarFisher.Console.Menu.Common
{
    public abstract class ParameterBase<T> : IParameter<T>
    {
        protected ParameterBase(bool isRequired = true)
        {
            IsRequired = isRequired;
        }

        public abstract Argument<T> GetArgument();

        public abstract void PrintInvalidArgumentMessage();

        public bool IsRequired { get; }

        protected static void Write(string text) => System.Console.Write(text);

        protected static void WriteLine(string text) => System.Console.WriteLine(text);

        protected static void WriteLine() => System.Console.WriteLine();

        protected static void WriteLineRed(string message)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(message);
            System.Console.ResetColor();
        }

        protected static void WaitForKeyPress() => System.Console.ReadKey();

        protected static string ReadLine() => System.Console.ReadLine()?.Trim();

        protected static void ClearLastLine()
        {
            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
            System.Console.Write(new string(' ', System.Console.BufferWidth));
            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
        }

        protected static void PrintInvalidArgumentMessage(string message)
        {
            WriteLine();
            WriteLineRed(message);
        }
    }
}
