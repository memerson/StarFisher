using System;
using System.Collections.Generic;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNominees.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.FixNominees.Commands
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

            var nominationList = Context.NominationListContext.NominationList;
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
            public Input(IReadOnlyCollection<PersonName> unrecognizedNomineeNames)
            {
                UnrecognizedNomineeNames = unrecognizedNomineeNames ??
                                           throw new ArgumentNullException(nameof(unrecognizedNomineeNames));
            }

            public IReadOnlyCollection<PersonName> UnrecognizedNomineeNames { get; }
        }
    }
}