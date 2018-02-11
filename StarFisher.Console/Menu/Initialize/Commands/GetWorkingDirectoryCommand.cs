using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Initialize.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.Initialize.Commands
{
    public class GetWorkingDirectoryCommand : InitializeCommandBase<DirectoryPath>
    {
        public GetWorkingDirectoryCommand(IStarFisherContext context) : base(context) { }

        protected override CommandResult<Output> RunCore(CommandInput.None input)
        {
            var getNewValueParameter = new WorkingDirectoryParameter();
            var currentWorkingDirectoryPath = Context.IsInitialized ? Context.WorkingDirectoryPath : null;
            return RunCoreHelper(@"working directory", currentWorkingDirectoryPath, getNewValueParameter);
        }
    }
}
