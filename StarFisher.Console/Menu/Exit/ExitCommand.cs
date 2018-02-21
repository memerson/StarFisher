using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;

namespace StarFisher.Console.Menu.Exit
{
    public class ExitCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Exit";

        public ExitCommand(IStarFisherContext context) : base(context, CommandTitle)
        {
        }

        public override bool GetCanRun()
        {
            return true;
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            Environment.Exit(0);
            return CommandOutput.None.Success;
        }
    }
}