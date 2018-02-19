using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Initialize.Parameters;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.Initialize.Commands
{
    public class GetPersonNameCommand : InitializeCommandBase<GetPersonNameCommand.Input, PersonName>
    {
        private readonly IGlobalAddressList _globalAddressList;

        public GetPersonNameCommand(IStarFisherContext context, IGlobalAddressList globalAddressList) : base(context)
        {
            _globalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
        }

        protected override CommandResult<Output> RunCore(Input input)
        {
            var getNewValueParameter = new PersonNameParameter(_globalAddressList, input.PersonTitle);
            return RunCoreHelper($@"{input.PersonTitle} name", input.PersonName, getNewValueParameter);
        }

        public class Input : CommandInput
        {
            public Input(string personTitle, PersonName personName = null)
            {
                if (string.IsNullOrWhiteSpace(personTitle))
                    throw new ArgumentException(nameof(personTitle));

                PersonTitle = personTitle;
                PersonName = personName;
            }

            public string PersonTitle { get; }

            public PersonName PersonName { get; }
        }
    }
}