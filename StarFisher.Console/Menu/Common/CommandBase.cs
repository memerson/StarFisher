using System;
using StarFisher.Console.Context;

namespace StarFisher.Console.Menu.Common
{
    public abstract class CommandBase<TInput, TOutput> : ICommand<TInput, TOutput>
        where TInput : CommandInput
        where TOutput : CommandOutput
    {
        protected CommandBase(IStarFisherContext context)
        {
            Context = context ?? StarFisherContext.Current;
        }

        protected CommandBase()
            : this(null) { }

        public CommandResult<TOutput> Run(TInput input)
        {
            try
            {
                var commandResult = RunCore(input);

                if (commandResult.ResultType != CommandResultType.Success)
                    return commandResult;

                Persist();

                return commandResult;
            }
            catch (Exception e)
            {
                Rollback();

                return CommandResult<TOutput>.Failure(e);
            }
        }

        protected IStarFisherContext Context { get; }

        protected static bool TryGetArgumentValue<T>(IParameter<T> parameter, out T argumentValue)
        {
            argumentValue = default(T);
            var argument = parameter.GetValidArgument();

            if (argument.ArgumentType == ArgumentType.Abort)
                return false;

            argumentValue = argument.Value;
            return true;
        }

        protected abstract CommandResult<TOutput> RunCore(TInput input);

        private void Rollback()
        {
            if (!Context.IsInitialized)
                return;

            if (Context.NominationListContext.HasNominationListLoaded)
                Context.NominationListContext.LoadLatestSnapshot();

            if (Context.AwardWinnerListContext.HasAwardWinnerListLoaded)
                Context.AwardWinnerListContext.LoadLatestSnapshot();
        }

        private void Persist()
        {
            if (!Context.IsInitialized)
                return;

            if (Context.NominationListContext.HasNominationListLoaded)
                Context.NominationListContext.SaveSnapshot();

            if (Context.AwardWinnerListContext.HasAwardWinnerListLoaded)
                Context.AwardWinnerListContext.SaveSnapshot();
        }
    }
}
