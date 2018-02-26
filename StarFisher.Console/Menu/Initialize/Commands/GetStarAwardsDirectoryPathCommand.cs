using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Initialize.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.Initialize.Commands
{
    public class GetStarAwardsDirectoryPathCommand : InitializeCommandBase<DirectoryPath>
    {
        public GetStarAwardsDirectoryPathCommand(IStarFisherContext context) : base(context)
        {
        }

        protected override CommandResult<Output> RunCore(CommandInput.None input)
        {
            var getNewValueParameter = new StarAwardsDirectoryPathParameter();
            var currentStarAwardsDirectoryPath = Context.IsInitialized ? Context.StarAwardsDirectoryPath : null;
            return RunCoreHelper(@"Star Awards directory", currentStarAwardsDirectoryPath, getNewValueParameter);
        }
    }
}