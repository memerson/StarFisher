using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNominees.Commands;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.FixNominees
{
    public class FixNomineesMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Fix nominee names, office locations, and email addresses";
        private readonly IGlobalAddressList _globalAddressList;

        public FixNomineesMenuItemCommand(IStarFisherContext context,
            IGlobalAddressList globalAddressList) :
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
                return CommandOutput.None.Failure(exception);

            if(!FixNomineeOfficeLocations(out exception))
                return CommandOutput.None.Failure(exception);

            if (!FixNomineeEmailAddresses(_globalAddressList, nominationList, out exception))
                return CommandOutput.None.Failure(exception);

            return CommandOutput.None.Success;
        }

        private bool FixNomineeNames(IGlobalAddressList globalAddressList, NominationList nominationList,
            out Exception exception)
        {
            var unrecognizedNomineeNames = GetUnrecognizedNomineeNames(globalAddressList, nominationList.Nominees);
            return FixNomineeNames(unrecognizedNomineeNames, out exception);
        }

        private bool FixNomineeEmailAddresses(IGlobalAddressList globalAddressList, NominationList nominationList,
            out Exception exception)
        {
            var unrecognizedEmailAddresses = GetUnrecognizedEmailAddresses(globalAddressList, nominationList.Nominees);
            return FixNomineeEmailAddresses(unrecognizedEmailAddresses, out exception);
        }

        private bool FixNomineeNames(IReadOnlyCollection<PersonName> unrecognizedNomineeNames, out Exception exception)
        {
            var fixNomineeNamesCommand = new FixNomineeNamesCommand(Context, _globalAddressList);
            var input = new FixNomineeNamesCommand.Input(unrecognizedNomineeNames);
            var commandResult = fixNomineeNamesCommand.Run(input);

            if (commandResult.ResultType == CommandResultType.Failure)
            {
                exception = commandResult.Exception;
                return false;
            }

            exception = null;
            return true;
        }

        private bool FixNomineeOfficeLocations(out Exception exception)
        {
            var fixNomineeOfficeLocationsCommand = new FixNomineeOfficeLocationsCommand(Context);
            var commandResult = fixNomineeOfficeLocationsCommand.Run(CommandInput.None.Instance);

            if (commandResult.ResultType == CommandResultType.Failure)
            {
                exception = commandResult.Exception;
                return false;
            }

            exception = null;
            return true;
        }

        private bool FixNomineeEmailAddresses(IReadOnlyCollection<EmailAddress> unrecognizedEmailAddresses, out Exception exception)
        {
            var fixNomineeEmailAddressesCommand = new FixNomineeEmailAddressesCommand(Context, _globalAddressList);
            var input = new FixNomineeEmailAddressesCommand.Input(unrecognizedEmailAddresses);
            var commandResult = fixNomineeEmailAddressesCommand.Run(input);

            if (commandResult.ResultType == CommandResultType.Failure)
            {
                exception = commandResult.Exception;
                return false;
            }

            exception = null;
            return true;
        }

        private static List<PersonName> GetUnrecognizedNomineeNames(IGlobalAddressList globalAddressList,
            IEnumerable<Person> nominees)
        {
            return nominees
                .Select(nominee => nominee.Name)
                .Distinct()
                .Where(name => !globalAddressList.GetPersonExists(name))
                .ToList();
        }

        private static List<EmailAddress> GetUnrecognizedEmailAddresses(IGlobalAddressList globalAddressList,
            IEnumerable<Person> nominees)
        {
            return nominees
                .Select(nominee => nominee.EmailAddress)
                .Distinct()
                .Where(emailAddress => !globalAddressList.GetPersonExists(emailAddress))
                .ToList();
        }
    }
}