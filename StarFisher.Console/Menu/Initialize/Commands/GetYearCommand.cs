using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Initialize.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.Initialize.Commands
{
    public class GetYearCommand : InitializeCommandBase<Year>
    {
        public GetYearCommand(IStarFisherContext context) : base(context)
        {
        }

        protected override CommandResult<Output> RunCore(CommandInput.None input)
        {
            var getNewValueParameter = new YearParameter();
            var currentYear = Context.IsInitialized ? Context.Year : null;
            return RunCoreHelper(@"year", currentYear, getNewValueParameter);
        }
    }
}