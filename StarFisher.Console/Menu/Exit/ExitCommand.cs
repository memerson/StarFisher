using System;
using StarFisher.Console.Menu.Common;

namespace StarFisher.Console.Menu.Exit
{
    public class ExitCommand : MenuItemCommandBase<CommandInput.None>
    {
        public ExitCommand() : base(@"Exit") { }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            Environment.Exit(0);
            return CommandOutput.None.Success;
        }
    }
}
