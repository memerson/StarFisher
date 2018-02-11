using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Initialize.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.Initialize.Commands
{
    public class GetQuarterCommand : InitializeCommandBase<Quarter>
    {
        public GetQuarterCommand(IStarFisherContext context) : base(context) { }

        protected override CommandResult<Output> RunCore(CommandInput.None input)
        {
            var getNewValueParameter = new QuarterParameter();
            var currentQuarter = Context.IsInitialized ? Context.Quarter : null;
            return RunCoreHelper(@"year", currentQuarter, getNewValueParameter);
        }
    }
}
