using System;
using System.Collections.Generic;
using StarFisher.Console.Menu.Common;

namespace StarFisher.Console.Menu.TopLevelMenu.Parameters
{
    public class MenuItemIndexParameter : ParameterBase<int>
    {
        private readonly IReadOnlyList<IMenuItemCommand> _menuItemCommands;

        public MenuItemIndexParameter(IReadOnlyList<IMenuItemCommand> menuItemCommands)
        {
            _menuItemCommands = menuItemCommands ?? throw new ArgumentNullException(nameof(menuItemCommands));
        }

        public override Argument<int> GetArgument()
        {
            System.Console.WriteLine();
            System.Console.WriteLine(@"What do you want to do? Enter the number of one of these menu items:");
            System.Console.WriteLine();

            // TODO: Fix this up so we can get the index but still show sequential numbers starting with 1.

            for (var i = 0; i < _menuItemCommands.Count; ++i)
            {
                var menuItemCommand = _menuItemCommands[i];

                if (!menuItemCommand.GetCanRun())
                    continue;

                System.Console.WriteLine($"{i + 1} {menuItemCommand.Title}");
            }

            System.Console.WriteLine();
            System.Console.Write(@"> ");

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not the number of any of the menu items.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out int argumentValue)
        {
            if (int.TryParse(input, out int value))
            {
                var index = value - 1;
                if (index >= 0 && index < _menuItemCommands.Count)
                {
                    argumentValue = index;
                    return true;
                }
            }

            argumentValue = default(int);
            return false;
        }
    }
}
