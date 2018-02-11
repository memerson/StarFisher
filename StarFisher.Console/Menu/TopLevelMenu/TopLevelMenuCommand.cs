using StarFisher.Console.Menu.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.TopLevelMenu.Parameters;

namespace StarFisher.Console.Menu.TopLevelMenu
{
    public class TopLevelMenuCommand : MenuItemCommandBase
    {
        private readonly IReadOnlyList<IMenuItemCommand> _menuItemCommands;
        private const string CommandTitle = @"Print the top-level menu";

        public TopLevelMenuCommand(IReadOnlyList<IMenuItemCommand> menuItemCommands)
            : base(CommandTitle)
        {
            _menuItemCommands = menuItemCommands ?? throw new ArgumentNullException(nameof(menuItemCommands));

            if (_menuItemCommands.Count == 0)
                throw new ArgumentException(nameof(menuItemCommands));
        }

        public override bool GetCanRun()
        {
            return _menuItemCommands.Any(mi => mi.GetCanRun());
        }

        public TopLevelMenuCommand(IStarFisherContext context) : base(context, CommandTitle) { }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var parameter = new MenuItemIndexParameter(_menuItemCommands);

            for (;;)
            {
                var argument = parameter.GetValidArgument();
                var menuItem = _menuItemCommands[argument.Value];
                menuItem.Run();
            }
        }
    }
}
