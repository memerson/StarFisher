using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Utilities;

namespace StarFisher.Console.Menu.Common
{
    public abstract class ParameterBase<T> : IParameter<T>
    {
        private readonly Dictionary<string, Argument<T>> _validInputs
            = new Dictionary<string, Argument<T>>(StringComparer.InvariantCultureIgnoreCase);

        protected ParameterBase(bool isRequired = true)
        {
            IsRequired = isRequired;
        }

        public Argument<T> GetValidArgument()
        {
            var argument = GetArgument();

            while (argument.ArgumentType == ArgumentType.Invalid)
            {
                PrintInvalidArgumentMessage();
                argument = GetArgument();
            }

            return argument;
        }

        public abstract Argument<T> GetArgument();

        public abstract void PrintInvalidArgumentMessage();

        protected void RegisterValidInput(string input, T argumentValue)
        {
            _validInputs[input] = Argument<T>.Valid(argumentValue);
        }

        protected void RegisterAbortInput(string input)
        {
            _validInputs[input] = Argument<T>.Abort;
        }

        protected bool GetIsAbortInput(out Argument<T> abortArgument)
        {
            var input = ReadInput();

            if (_validInputs.TryGetValue(input, out Argument<T> argument) &&
                argument.ArgumentType == ArgumentType.Abort)
            {
                abortArgument = argument;
                return true;
            }

            abortArgument = null;
            return false;
        }

        protected Argument<T> GetRegisteredValidInputArgument()
        {
            var input = ReadInput();
            return TryGetRegisteredValidInputArgument(input, out Argument<T> argument) ? argument : Argument<T>.Invalid;
        }

        protected bool TryGetRegisteredValidInputArgument(string input, out Argument<T> argument)
        {
            argument = null;
            return !string.IsNullOrWhiteSpace(input) && _validInputs.TryGetValue(input, out argument);
        }

        protected Argument<T> GetArgumentFromInputIfValid()
        {
            var input = ReadInput();
            return GetArgumentFromInputIfValid(input);
        }

        protected Argument<T> GetArgumentFromInputIfValid(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Argument<T>.Invalid;

            if (TryGetRegisteredValidInputArgument(input, out Argument<T> argument))
                return argument;

            return TryParseArgumentValueFromInput(input, out T argumentValue)
                ? Argument<T>.Valid(argumentValue)
                : Argument<T>.Invalid;
        }

        protected virtual bool TryParseArgumentValueFromInput(string input, out T argumentValue)
        {
            throw new NotImplementedException();
        }

        public bool IsRequired { get; }

        protected static void Write(string text) => System.Console.Write(text);

        protected static void WriteRed(string text)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.Red))
                Write(text);
        }

        protected static void WriteLine(string text, params string[] redTokens)
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

        protected static void WriteLine() => System.Console.WriteLine();

        protected static void WriteLineRed(string text)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.Red))
                WriteLine(text);
        }

        protected static void WriteLineBlue(string text, params string[] redTokens)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.DarkCyan))
                WriteLine(text, redTokens);
        }

        protected static void WaitForKeyPress() => System.Console.ReadKey();

        protected static string ReadLine() => System.Console.ReadLine()?.Trim();

        protected static string ReadInput() => ReadLine().Replace("'", string.Empty);

        protected static void ClearLastLine()
        {
            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
            System.Console.Write(new string(' ', System.Console.BufferWidth));
            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
        }

        protected static void PrintInvalidArgumentMessage(string text)
        {
            WriteLine();
            WriteLineRed(text);
        }
    }
}
