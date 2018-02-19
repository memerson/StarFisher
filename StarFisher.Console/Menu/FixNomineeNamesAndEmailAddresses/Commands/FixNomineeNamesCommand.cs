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
    public class FixNomineeNamesCommand : CommandBase<FixNomineeNamesCommand.Input, CommandOutput.None>
    {
        private readonly IGlobalAddressList _globalAddressList;

        public FixNomineeNamesCommand(IStarFisherContext context, IGlobalAddressList globalAddressList) : base(context)
        {
            _globalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
        }

        protected override CommandResult<CommandOutput.None> RunCore(Input input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var nominationList = input.NominationList;
            var unrecognizedNomineeNames = input.UnrecognizedNomineeNames;

            for (;;)
            {
                var nomineeParameter =
                    new NomineeToChangeNameParameter(nominationList.Nominees, unrecognizedNomineeNames);

                if (!TryGetArgumentValue(nomineeParameter, out Person nomineeToChange))
                    break;

                var newNomineeNameParameter = new NewNomineeNameParameter(_globalAddressList, nomineeToChange);
                if (!TryGetArgumentValue(newNomineeNameParameter, out PersonName newNomineeName))
                    continue;

                nominationList.UpdateNomineeName(nomineeToChange, newNomineeName);
            }

            return CommandOutput.None.Success;
        }

        public class Input : CommandInput
        {
            public Input(NominationList nominationList, IReadOnlyCollection<PersonName> unrecognizedNomineeNames)
            {
                NominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
                UnrecognizedNomineeNames = unrecognizedNomineeNames ??
                                           throw new ArgumentNullException(nameof(unrecognizedNomineeNames));
            }

            public NominationList NominationList { get; }

            public IReadOnlyCollection<PersonName> UnrecognizedNomineeNames { get; }
        }
    }
}