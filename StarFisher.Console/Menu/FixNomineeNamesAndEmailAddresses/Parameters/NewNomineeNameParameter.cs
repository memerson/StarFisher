using System;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public class NewNomineeNameParameter : PersonNameParameterBase
    {
        private readonly Person _nominee;

        public NewNomineeNameParameter(IGlobalAddressList globalAddressList, Person nominee)
            : base(globalAddressList)
        {
            _nominee = nominee ?? throw new ArgumentNullException(nameof(nominee));

            RegisterAbortInput(@"done");
        }

        protected override string GetCallToActionText()
        {
            return $@"Enter the new name for the nominee currently named {_nominee.Name.FullName} from {
                    _nominee.OfficeLocation.ConciseName
                }, or enter 'done' if you don't want to change it.";
        }
    }
}