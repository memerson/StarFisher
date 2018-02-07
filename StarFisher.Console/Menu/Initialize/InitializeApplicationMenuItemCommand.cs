using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;

namespace StarFisher.Console.Menu.Initialize
{
    public class InitializeApplicationMenuItemCommand : MenuItemCommandBase
    {
        public InitializeApplicationMenuItemCommand(IStarFisherContext context, string title) 
            : base(context, title)
        {
        }

        public InitializeApplicationMenuItemCommand(string title)
            : base(title)
        {
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            throw new NotImplementedException();
        }

        public override bool GetCanRun() => true;
    }
}
