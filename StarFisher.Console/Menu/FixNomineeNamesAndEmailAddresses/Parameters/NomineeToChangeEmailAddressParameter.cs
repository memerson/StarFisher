using System;
using System.Collections.Generic;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public class NomineeToChangeEmailAddressParameter : NomineeParameterBase
    {
        private readonly HashSet<EmailAddress> _unrecognizedEmailAddresses;

        public NomineeToChangeEmailAddressParameter(IReadOnlyCollection<Person> allNominees, IReadOnlyCollection<EmailAddress> unrecognizedEmailAddresses)
            : base(allNominees)
        {
            _unrecognizedEmailAddresses =
                new HashSet<EmailAddress>(unrecognizedEmailAddresses ??
                                        throw new ArgumentNullException(nameof(unrecognizedEmailAddresses)));

            RegisterAbortInput(@"done");
        }

        protected override void WriteListIntroduction()
        {
            WriteLineBlue(
                @"Here are the nominee email addresses. Email addresses in red aren't in the global address list and so are probably wrong.",
                @"red");
        }

        protected override string GetListItemLabel(Person listItem)
        {
            return $@"{listItem.EmailAddress} ({listItem.Name.FullName} from {listItem.OfficeLocation.ConciseName}";
        }

        protected override void WriteListItem(Person listItem, string listItemText)
        {
            if (_unrecognizedEmailAddresses.Contains(listItem.EmailAddress))
                WriteLineRed(listItemText);
            else
                WriteLine(listItemText);
        }

        protected override string GetSelectionInstructions()
        {
            return @"Enter the number of the email address you want to change, or enter 'done' if you don't want to modify any email addresses.";
        }
    }
}
