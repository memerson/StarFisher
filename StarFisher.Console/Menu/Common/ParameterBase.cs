using System;
using System.Collections.Generic;

namespace StarFisher.Console.Menu.Common
{
    public abstract class ParameterBase<T> : IParameter<T>
    {
        private readonly IStarFisherConsole _console = StarFisherConsole.Instance;
        private readonly bool _printCommandTitle;

        private readonly Dictionary<string, Argument<T>> _validInputs
            = new Dictionary<string, Argument<T>>(StringComparer.InvariantCultureIgnoreCase);

        protected ParameterBase(bool printCommandTitle = true)
        {
            _printCommandTitle = printCommandTitle;
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

        public abstract Argument<T> GetArgumentCore();

        public abstract void PrintInvalidArgumentMessage();

        public Argument<T> GetArgument()
        {
            _console.ResetConsole(_printCommandTitle);
            return GetArgumentCore();
        }

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

        protected void WriteLine(string text, params string[] redTokens)
        {
            _console.WriteLine(text, redTokens);
        }

        protected void WriteLine()
        {
            _console.WriteLine();
        }

        protected void WriteLineRed(string text)
        {
            _console.WriteLineRed(text);
        }

        protected void WriteIntroduction(string text, params string[] redTokens)
        {
            _console.WriteLineBlue(text, redTokens);
        }

        protected void WriteCallToAction(string text)
        {
            _console.WriteLineYellow(text);
        }

        protected void WriteInputPrompt()
        {
            _console.WriteInputPrompt();
        }

        protected void WaitForKeyPress()
        {
            _console.WaitForKeyPress();
        }

        protected string ReadLine()
        {
            return _console.ReadLine();
        }

        protected string ReadInput()
        {
            return ReadLine().Replace("'", string.Empty);
        }

        protected void ClearLastLine()
        {
            _console.ClearLastLine();
        }

        protected void PrintInvalidArgumentMessage(string text)
        {
            _console.SetErrorMessage(text);
        }
    }
}