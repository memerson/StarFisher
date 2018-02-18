using System;
using System.Collections.Generic;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Commands
{
    public class FixNomineeEmailAddressesCommand : CommandBase<FixNomineeEmailAddressesCommand.Input, CommandOutput.None>
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

            var nominationList = input.NominationList;
            var unrecognizedEmailAddresses = input.UnrecognizedEmailAddresses;

            for (; ; )
            {
                var nomineeParameter = new NomineeToChangeEmailAddressParameter(nominationList.Nominees, unrecognizedEmailAddresses);

                if (!TryGetArgumentValue(nomineeParameter, out Person personToChange))
                    break;

                var newNomineeNameParameter = new NewNomineeEmailAddressParameter(_globalAddressList, personToChange);
                if (!TryGetArgumentValue(newNomineeNameParameter, out EmailAddress newEmailAddress))
                    continue;

                nominationList.UpdateNomineeEmailAddress(personToChange, newEmailAddress);

                if (!Context.AwardWinnerListContext.HasAwardWinnerListLoaded)
                    continue;

                var awardWinnerList = Context.AwardWinnerListContext.AwardWinnerList;
                awardWinnerList.UpdateWinnerEmailAddress(personToChange, newEmailAddress);
            }

            return CommandOutput.None.Success;
        }

        public class Input : CommandInput
        {
            public Input(NominationList nominationList, IReadOnlyCollection<EmailAddress> unrecognizedEmailAddresses)
            {
                NominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
                UnrecognizedEmailAddresses = unrecognizedEmailAddresses ?? throw new ArgumentNullException(nameof(unrecognizedEmailAddresses));
            }

            public NominationList NominationList { get; }

            public IReadOnlyCollection<EmailAddress> UnrecognizedEmailAddresses { get; }
        }
    }
}
