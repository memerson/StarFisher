using System;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.Initialize.Parameters
{
    public class EmailAddressParameter : EmailAddressParameterBase
    {
        private readonly PersonName _personName;

        public EmailAddressParameter(IGlobalAddressList globalAddressList, PersonName personName)
            : base(globalAddressList, personName)
        {
            _personName = personName ?? throw new ArgumentNullException(nameof(personName));

            RegisterAbortInput(@"stop");
        }

        protected override string GetCallToActionText()
        {
            return $@"Enter {
                    _personName.FullName
                }'s email address, or enter 'stop' to stop the initialization workflow.";
        }
    }
}