using System;
using System.Collections.Generic;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNominees.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.FixNominees.Commands
{
    public class FixNomineeEmailAddressesCommand : CommandBase<FixNomineeEmailAddressesCommand.Input, CommandOutput.None
    >
    {
        private readonly IGlobalAddressList _globalAddressList;

        public FixNomineeEmailAddressesCommand(IStarFisherContext context, IGlobalAddressList globalAddressList)
            : base(context)
        {
            _globalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
        }

        protected override CommandResult<CommandOutput.None> RunCore(Input input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var nominationList = Context.NominationListContext.NominationList;
            var unrecognizedEmailAddresses = input.UnrecognizedEmailAddresses;

            for (;;)
            {
                var nomineeParameter =
                    new NomineeToChangeEmailAddressParameter(nominationList.Nominees, unrecognizedEmailAddresses);

                if (!TryGetArgumentValue(nomineeParameter, out Person nomineeToChange))
                    break;

                var newNomineeNameParameter = new NewNomineeEmailAddressParameter(_globalAddressList, nomineeToChange);
                if (!TryGetArgumentValue(newNomineeNameParameter, out EmailAddress newEmailAddress))
                    continue;

                nominationList.UpdateNomineeEmailAddress(nomineeToChange, newEmailAddress);
            }

            return CommandOutput.None.Success;
        }

        public class Input : CommandInput
        {
            public Input(IReadOnlyCollection<EmailAddress> unrecognizedEmailAddresses)
            {
                UnrecognizedEmailAddresses = unrecognizedEmailAddresses ??
                                             throw new ArgumentNullException(nameof(unrecognizedEmailAddresses));
            }

            public IReadOnlyCollection<EmailAddress> UnrecognizedEmailAddresses { get; }
        }
    }
}