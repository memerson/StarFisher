using StarFisher.Console.Menu.Common;
using System;
using System.Collections.Generic;
using StarFisher.Console.Menu.TopLevelMenu.Parameters;

namespace StarFisher.Console.Menu.TopLevelMenu
{
    public class TopLevelMenuCommand : MenuItemCommandBase<TopLevelMenuCommand.Input>
    {
        public TopLevelMenuCommand()
            : base(@"Print the top-level menu") { }

        protected override CommandResult<CommandOutput.None> RunCore(Input input)
        {
            var parameter = new MenuItemIndexParameter(input.MenuItems);

            for (;;)
            {
                var argument = parameter.GetArgument();

                while (argument.ArgumentType == ArgumentType.Invalid)
                {
                    parameter.PrintInvalidArgumentMessage();
                    argument = parameter.GetArgument();
                }

                var menuItem = input.MenuItems[argument.Value];
                menuItem.Run();
            }
        }

        public class Input : CommandInput
        {
            public Input(IReadOnlyList<MenuItem> menuItems)
            {
                MenuItems = menuItems ?? throw new ArgumentNullException(nameof(menuItems));

                if (MenuItems.Count == 0)
                    throw new ArgumentException(nameof(menuItems));
            }

            public IReadOnlyList<MenuItem> MenuItems { get; }
        }
    }
}
