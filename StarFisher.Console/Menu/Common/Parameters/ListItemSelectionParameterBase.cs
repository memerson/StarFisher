using System;
using System.Collections.Generic;

namespace StarFisher.Console.Menu.Common.Parameters
{
    public abstract class ListItemSelectionParameterBase<T> : ParameterBase<T>
    {
        private readonly IReadOnlyList<T> _list;
        private readonly string _itemsDescription;

        protected ListItemSelectionParameterBase(IReadOnlyList<T> list, string itemsDescription)
        {
            if (string.IsNullOrWhiteSpace(itemsDescription))
                throw new ArgumentException(nameof(itemsDescription));

            _list = list ?? throw new ArgumentNullException(nameof(list));
            _itemsDescription = itemsDescription;
        }

        public override Argument<T> GetArgument()
        {
            if (_list.Count == 0)
            {
                WriteLine();
                WriteLine($@"There are no {_itemsDescription}.");
                WriteLine();
                return Argument<T>.Abort;
            }
            
            SolicitInput();

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage($@"That's not a valid selection. Enter one of the numbers next to one of the {_itemsDescription}.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out T argumentValue)
        {
            if (int.TryParse(input, out int nameId))
            {
                var index = nameId - 1;
                if (index >= 0 && index < _list.Count)
                {
                    argumentValue = _list[index];
                    return true;
                }
            }

            argumentValue = default(T);
            return false;
        }

        protected virtual void WriteListIntroduction()
        {
            WriteLineBlue($@"Here are the {_itemsDescription}:");
        }

        protected abstract string GetListItemLabel(T listItem);

        protected virtual void WriteListItem(T listItem, string listItemText)
        {
            WriteLine(listItemText);
        }

        protected virtual void WriteSelectionInstructions()
        {
        }

        private void SolicitInput()
        {
            WriteLine();
            WriteListIntroduction();
            WriteLine();
            WriteLine();

            var itemsWritten = 0;

            for (var i = 0; i < _list.Count; ++i)
            {
                var listItem = _list[i];
                var listItemLabel = GetListItemLabel(listItem);

                if (string.IsNullOrWhiteSpace(listItemLabel))
                    continue;

                var listItemText = $@"{i + 1, 5}: {listItemLabel}";

                if (itemsWritten != 0 && itemsWritten % 20 == 0)
                {
                    Write(@"Press any key to continue.");
                    WaitForKeyPress();
                    ClearLastLine();
                }

                WriteListItem(listItem, listItemText);
                ++itemsWritten;
            }

            WriteLine();
            WriteSelectionInstructions();
            Write(@"> ");
        }
    }
}
