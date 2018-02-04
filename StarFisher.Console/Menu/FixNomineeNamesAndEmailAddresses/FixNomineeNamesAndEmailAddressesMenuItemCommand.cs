using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Commands;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses
{
    public class FixNomineeNamesAndEmailAddressesMenuItemCommand : MenuItemCommandBase<FixNomineeNamesAndEmailAddressesMenuItemCommand.Input>
    {
        public FixNomineeNamesAndEmailAddressesMenuItemCommand()
            : base(@"Fix nominee names and email addresses") { }

        protected override CommandResult<CommandOutput.None> RunCore(Input input)
        {
            Exception exception;

            if (!FixNomineeNames(input.GlobalAddressList, input.NominationList, out exception))
                return CommandResult<CommandOutput.None>.Failure(exception);

            if(!FixNomineeEmailAddresses(input.GlobalAddressList, input.NominationList, out exception))
                return CommandResult<CommandOutput.None>.Failure(exception);

            return CommandOutput.None.Success;
        }

        private static bool FixNomineeNames(IGlobalAddressList globalAddressList, NominationList nominationList, out Exception exception)
        {
            var unrecognizedNomineeNames = GetUnrecognizedNomineeNames(globalAddressList, nominationList.Nominees);
            return FixNomineeNames(nominationList, unrecognizedNomineeNames, out exception);
        }

        private bool FixNomineeEmailAddresses(IGlobalAddressList globalAddressList, NominationList nominationList, out Exception exception)
        {
            var unrecognizedEmailAddresses = GetUnrecognizedEmailAddresses(globalAddressList, nominationList.Nominees);
            return FixNomineeEmailAddresses(nominationList, unrecognizedEmailAddresses, out exception);
        }

        private static bool FixNomineeNames(NominationList nominationList, IReadOnlyCollection<PersonName> unrecognizedNomineeNames, out Exception exception)
        {
            var fixNomineeNamesCommand = new FixNomineeNamesCommand();
            var input = new FixNomineeNamesCommand.Input(nominationList, unrecognizedNomineeNames);
            var commandResult = fixNomineeNamesCommand.Run(input);

            if (commandResult.ResultType == CommandResultType.Failure)
            {
                exception = commandResult.Exception;
                return false;
            }

            exception = null;
            return true;
        }

        private static bool FixNomineeEmailAddresses(NominationList nominationList, IReadOnlyCollection<EmailAddress> unrecognizedEmailAddresses, out Exception exception)
        {
            var fixNomineeEmailAddressesCommand = new FixNomineeEmailAddressesCommand();
            var input = new FixNomineeEmailAddressesCommand.Input(nominationList, unrecognizedEmailAddresses);
            var commandResult = fixNomineeEmailAddressesCommand.Run(input);

            if (commandResult.ResultType == CommandResultType.Failure)
            {
                exception = commandResult.Exception;
                return false;
            }

            exception = null;
            return true;
        }

        private static List<PersonName> GetUnrecognizedNomineeNames(IGlobalAddressList globalAddressList, IEnumerable<Person> nominees)
        {
            return nominees
                .Select(nominee => nominee.Name)
                .Distinct()
                .Where(name => !globalAddressList.GetPersonExists(name))
                .ToList();
        }

        private static List<EmailAddress> GetUnrecognizedEmailAddresses(IGlobalAddressList globalAddressList, IEnumerable<Person> nominees)
        {
            return nominees
                .Select(nominee => nominee.EmailAddress)
                .Distinct()
                .Where(emailAddress => !globalAddressList.GetPersonExists(emailAddress))
                .ToList();
        }

        public class Input : CommandInput
        {
            public Input(IGlobalAddressList globalAddressList, NominationList nominationList)
            {
                GlobalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
                NominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
            }

            public IGlobalAddressList GlobalAddressList { get; }

            public NominationList NominationList { get; }
        }
    }
}
