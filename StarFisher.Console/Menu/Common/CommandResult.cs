using System;

namespace StarFisher.Console.Menu.Common
{
    public class CommandResult<T> 
        where T : CommandOutput
    {
        private CommandResult(CommandResultType resultType, T output, Exception exception)
        {
            ResultType = resultType;
            Output = output;
            Exception = exception;
        }

        public static CommandResult<T> Failure(Exception exception)
        {
            return new CommandResult<T>(CommandResultType.Failure, default(T), exception);
        }

        public static CommandResult<T> Success(T state)
        {
            return new CommandResult<T>(CommandResultType.Success, state, null);
        }

        public static CommandResult<T> Abort()
        {
            return new CommandResult<T>(CommandResultType.Abort, default(T), null);
        }

        public CommandResultType ResultType { get; }

        public T Output { get; }

        public Exception Exception { get; }
    }
}
