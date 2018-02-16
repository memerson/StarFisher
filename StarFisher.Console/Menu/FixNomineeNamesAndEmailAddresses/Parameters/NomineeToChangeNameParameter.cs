using System;
using System.Collections.Generic;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public class NomineeToChangeNameParameter : NomineeParameterBase
    {
        private readonly HashSet<PersonName> _unrecognizedNomineeNames;

        public NomineeToChangeNameParameter(IReadOnlyCollection<Person> allNominees, IReadOnlyCollection<PersonName> unrecognizedNomineeNames)
            : base(allNominees)
        {
            _unrecognizedNomineeNames =
                new HashSet<PersonName>(unrecognizedNomineeNames ??
                                        throw new ArgumentNullException(nameof(unrecognizedNomineeNames)));

            RegisterAbortInput(@"done");
        }

        protected override void WriteListIntroduction()
        {
            WriteIntroduction(
                @"Here are the nominee names. Any names in red aren't in the global address list and so are probably wrong.",
                @"red");
        }

        protected override string GetListItemLabel(Person listItem)
        {
            return $@"{listItem.Name.FullName, -25} {listItem.OfficeLocation.ConciseName}";
        }

        protected override void WriteListItem(Person listItem, string listItemText)
        {
            if (_unrecognizedNomineeNames.Contains(listItem.Name))
                WriteLineRed(listItemText);
            else
                WriteLine(listItemText);
        }

        protected override void WriteCallToAction()
        {
            WriteCallToAction(@"Enter the number of the name you want to change, or enter 'done' if you don't want to modify any names.");
        }
    }
}
