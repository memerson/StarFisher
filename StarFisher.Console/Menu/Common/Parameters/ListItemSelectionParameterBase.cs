using System;
using System.Collections.Generic;

namespace StarFisher.Console.Menu.Common.Parameters
{
    public abstract class ListItemSelectionParameterBase<T> : ParameterBase<T>
    {
        private readonly string _itemsDescription;

        protected ListItemSelectionParameterBase(IReadOnlyList<T> listItems, string itemsDescription,
            bool printCommandTitle = true)
            : base(printCommandTitle)
        {
            if (string.IsNullOrWhiteSpace(itemsDescription))
                throw new ArgumentException(nameof(itemsDescription));

            ListItems = listItems ?? throw new ArgumentNullException(nameof(listItems));
            _itemsDescription = itemsDescription;
        }

        protected IReadOnlyList<T> ListItems { get; }

        public override Argument<T> GetArgumentCore()
        {
            if (ListItems.Count == 0)
            {
                WriteLine();
                WriteLine($@"There are no {_itemsDescription}. Press any key to continue.");
                WriteInputPrompt();
                WaitForKeyPress();
                return Argument<T>.Abort;
            }

            SolicitInput();

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(
                $@"That's not a valid selection. Enter one of the numbers next to one of the {_itemsDescription}.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out T argumentValue)
        {
            if (int.TryParse(input, out int nameId))
            {
                var index = nameId - 1;
                if (index >= 0 && index < ListItems.Count)
                {
                    argumentValue = ListItems[index];
                    return true;
                }
            }

            argumentValue = default(T);
            return false;
        }

        protected virtual void WriteListIntroduction()
        {
            WriteIntroduction($@"Here are the {_itemsDescription}:");
        }

        protected abstract string GetListItemLabel(T listItem);

        protected virtual void WriteListItem(T listItem, string listItemText)
        {
            WriteLine(listItemText);
        }

        protected virtual void WriteCallToAction()
        {
        }

        private void SolicitInput()
        {
            WriteLine();
            WriteListIntroduction();
            WriteLine();
            WriteLine();

            for (var i = 0; i < ListItems.Count; ++i)
            {
                var listItem = ListItems[i];
                var listItemLabel = GetListItemLabel(listItem);

                if (string.IsNullOrWhiteSpace(listItemLabel))
                    continue;

                var listItemText = $@"{i + 1,5}: {listItemLabel}";

                WriteListItem(listItem, listItemText);
            }

            WriteLine();
            WriteCallToAction();
            WriteInputPrompt();
        }
    }
}