
using System;

namespace StarFisher.Console.Commands.Common
{
    public class CommandResult
    {
        public static readonly CommandResult NotAttempted = new CommandResult(false, false);
        public static readonly CommandResult Success = new CommandResult(true, true);

        private CommandResult(bool executionAttempted, bool executionSucceeded, string errorText = null)
        {
            ExecutionAttempted = executionAttempted;
            ExecutionSucceeded = executionSucceeded;
            ExecutionFailed = errorText != null;
            ErrorText = errorText;
        }

        public static CommandResult Error(string errorText)
        {
            if(string.IsNullOrWhiteSpace(errorText))
                throw new ArgumentException(nameof(errorText));

            return new CommandResult(true, false, errorText);
        }

        public bool ExecutionAttempted { get; }

        public bool ExecutionSucceeded { get; }

        public bool ExecutionFailed { get; }

        public string ErrorText { get; }
    }
}
