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

            for (var i = 0; i < _menuItemCommands.Count; ++i)
            {
                var menuItem = _menuItemCommands[i];
                System.Console.WriteLine($"{i + 1} {menuItem.Title}");
            }

            System.Console.WriteLine();
            System.Console.Write(@"> ");

            if(!int.TryParse(ReadInput(), out int input))
                return Argument<int>.Invalid;

            var index = input - 1;

            if(index < 0 || index >= _menuItemCommands.Count)
                return Argument<int>.Invalid;

            return Argument<int>.Valid(index);
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not the number of any of the menu items.");
        }
    }
}
