using System;

namespace StarFisher.Console.Menu.Common
{
    public abstract class CommandBase<TInput, TOutput> : ICommand<TInput, TOutput>
        where TInput : CommandInput
        where TOutput : CommandOutput
    {
        protected CommandBase(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException(nameof(title));

            Title = title;
        }

        public string Title { get; }

        public CommandResult<TOutput> Run(TInput input)
        {
            try
            {
                return RunCore(input);
            }
            catch (Exception e)
            {
                return CommandResult<TOutput>.Failure(e);
            }
        }

        protected abstract CommandResult<TOutput> RunCore(TInput input);
    }
}
