using System;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.FixNominees.Parameters
{
    public class NewNomineeEmailAddressParameter : EmailAddressParameterBase
    {
        private readonly Person _nominee;

        public NewNomineeEmailAddressParameter(IGlobalAddressList globalAddressList, Person nominee)
            : base(globalAddressList, nominee?.Name)
        {
            _nominee = nominee ?? throw new ArgumentNullException(nameof(nominee));

            RegisterAbortInput(@"done");
        }

        protected override string GetCallToActionText()
        {
            return $@"Enter the email address for the nominee named {_nominee.Name.FullName} from {
                    _nominee.OfficeLocation.Name
                }, or enter 'done' if you don't want to change it.";
        }
    }
}