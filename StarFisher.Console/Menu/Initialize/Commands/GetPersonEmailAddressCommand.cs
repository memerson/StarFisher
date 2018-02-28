using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Initialize.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.Initialize.Commands
{
    public class GetPersonEmailAddressCommand : InitializeCommandBase<GetPersonEmailAddressCommand.Input, EmailAddress>
    {
        private readonly IGlobalAddressList _globalAddressList;

        public GetPersonEmailAddressCommand(IStarFisherContext context,
            IGlobalAddressList globalAddressList) : base(context)
        {
            _globalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
        }

        protected override CommandResult<Output> RunCore(Input input)
        {
            var getNewValueParameter = new EmailAddressParameter(_globalAddressList, input.PersonName);
            return RunCoreHelper($@"{input.PersonName}'s email address", input.EmailAddress, getNewValueParameter);
        }

        public class Input : CommandInput
        {
            public Input(PersonName personName, EmailAddress emailAddress = null)
            {
                PersonName = personName ?? throw new ArgumentNullException(nameof(personName));
                EmailAddress = emailAddress;
            }

            public PersonName PersonName { get; }

            public EmailAddress EmailAddress { get; }
        }
    }
}