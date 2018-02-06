using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Commands;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses
{
    public class FixNomineeNamesAndEmailAddressesMenuItemCommand : MenuItemCommandBase
    {
        private readonly IGlobalAddressList _globalAddressList;
        private const string CommandTitle = @"Fix nominee names and email addresses";

        public FixNomineeNamesAndEmailAddressesMenuItemCommand(IGlobalAddressList globalAddressList) : 
            base(CommandTitle)
        {
            _globalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
        }

        public FixNomineeNamesAndEmailAddressesMenuItemCommand(IStarFisherContext context, IGlobalAddressList globalAddressList) :
            base(context, CommandTitle)
        {
            _globalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized && Context.NominationListContext.HasNominationListLoaded;
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;

            if (!FixNomineeNames(_globalAddressList, nominationList, out Exception exception))
                return CommandResult<CommandOutput.None>.Failure(exception);

            if (!FixNomineeEmailAddresses(_globalAddressList, nominationList, out exception))
                return CommandResult<CommandOutput.None>.Failure(exception);

            return CommandOutput.None.Success;
        }

        private bool FixNomineeNames(IGlobalAddressList globalAddressList, NominationList nominationList, out Exception exception)
        {
            var unrecognizedNomineeNames = GetUnrecognizedNomineeNames(globalAddressList, nominationList.Nominees);
            return FixNomineeNames(nominationList, unrecognizedNomineeNames, out exception);
        }

        private bool FixNomineeEmailAddresses(IGlobalAddressList globalAddressList, NominationList nominationList, out Exception exception)
        {
            var unrecognizedEmailAddresses = GetUnrecognizedEmailAddresses(globalAddressList, nominationList.Nominees);
            return FixNomineeEmailAddresses(nominationList, unrecognizedEmailAddresses, out exception);
        }

        private bool FixNomineeNames(NominationList nominationList, IReadOnlyCollection<PersonName> unrecognizedNomineeNames, out Exception exception)
        {
            var fixNomineeNamesCommand = new FixNomineeNamesCommand(Context);
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

        private bool FixNomineeEmailAddresses(NominationList nominationList, IReadOnlyCollection<EmailAddress> unrecognizedEmailAddresses, out Exception exception)
        {
            var fixNomineeEmailAddressesCommand = new FixNomineeEmailAddressesCommand(Context);
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
    }
}
