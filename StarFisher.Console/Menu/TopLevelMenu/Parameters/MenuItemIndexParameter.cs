using System.Collections.Generic;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Common.Parameters;

namespace StarFisher.Console.Menu.TopLevelMenu.Parameters
{
    public class MenuItemIndexParameter : ListItemSelectionParameterBase<IMenuItemCommand>
    {
        public MenuItemIndexParameter(IReadOnlyList<IMenuItemCommand> menuItemCommands)
            : base(menuItemCommands, @"menu items", false)
        {
        }

        protected override string GetListItemLabel(IMenuItemCommand listItem)
        {
            return listItem.GetCanRun() ? listItem.Title : null;
        }

        protected override void WriteCallToAction()
        {
            WriteCallToAction(@"Enter one of the the numbers next to one of the menu items.");
        }

        protected override void WriteListIntroduction()
        {
            WriteIntroduction(@"What do you want to do?");
        }
    }
}