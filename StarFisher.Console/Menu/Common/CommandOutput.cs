using System;

namespace StarFisher.Console.Menu.Common
{
    public abstract class CommandOutput
    {
        public sealed class None : CommandOutput
        {
            public static readonly None Instance = new None();
            public static readonly CommandResult<None> Success = CommandResult<None>.Success(Instance);
            public static readonly CommandResult<None> Abort = CommandResult<None>.Abort();

            private None()
            {
            }

            public static CommandResult<None> Failure(Exception exception)
            {
                return CommandResult<None>.Failure(exception);
            }
        }
    }
}