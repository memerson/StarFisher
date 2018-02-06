using System;
using StarFisher.Console.Context;

namespace StarFisher.Console.Menu.Common
{
    public abstract class CommandBase<TInput, TOutput> : ICommand<TInput, TOutput>
        where TInput : CommandInput
        where TOutput : CommandOutput
    {
        protected CommandBase(IStarFisherContext context, string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException(nameof(title));

            Title = title;
            Context = context ?? StarFisherContext.Current;
        }

        protected CommandBase(string title)
            : this(null, title) { }

        public string Title { get; }

        public CommandResult<TOutput> Run(TInput input)
        {
            try
            {
                var commandResult = RunCore(input);

                if (commandResult.ResultType != CommandResultType.Success)
                    return commandResult;

                if (Context.NominationListContext.HasNominationListLoaded)
                    Context.NominationListContext.SaveSnapshot();

                if(Context.AwardWinnerListContext.HasAwardWinnerListLoaded)
                    Context.AwardWinnerListContext.SaveSnapshot();

                return commandResult;
            }
            catch (Exception e)
            {
                return CommandResult<TOutput>.Failure(e);
            }
        }

        protected IStarFisherContext Context { get; }

        protected abstract CommandResult<TOutput> RunCore(TInput input);
    }
}
