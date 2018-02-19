using System;
using System.Collections.Generic;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Initialize.Commands;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.Initialize
{
    public class InitializeApplicationMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Initialize StarFisher";
        private readonly IConfigurationStorage _configurationStorage;
        private readonly IGlobalAddressList _globalAddressList;

        public InitializeApplicationMenuItemCommand(IStarFisherContext context, IGlobalAddressList globalAddressList,
            IConfigurationStorage configurationStorage)
            : base(context, CommandTitle)
        {
            _globalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
            _configurationStorage = configurationStorage ??
                                    throw new ArgumentNullException(nameof(configurationStorage));
        }

        public override bool GetCanRun()
        {
            return true;
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var workingDirectoryPath = GetCommandResult(new GetWorkingDirectoryCommand(Context),
                out CommandResult<CommandOutput.None> unsuccessfulResult);
            if (workingDirectoryPath == null)
                return unsuccessfulResult;

            var year = GetCommandResult(new GetYearCommand(Context), out unsuccessfulResult);
            if (year == null)
                return unsuccessfulResult;

            var quarter = GetCommandResult(new GetQuarterCommand(Context), out unsuccessfulResult);
            if (quarter == null)
                return unsuccessfulResult;

            var eiaChairPerson = GetEiaChairPerson(out unsuccessfulResult);
            if (eiaChairPerson == null)
                return unsuccessfulResult;

            var hrPeople = GetHrPeople(out unsuccessfulResult);
            if (hrPeople == null)
                return unsuccessfulResult;

            var luncheonPlannerPeople = GetLuncheonPlannerPeople(out unsuccessfulResult);
            if (luncheonPlannerPeople == null)
                return unsuccessfulResult;

            var certificatePrinterPerson = GetCertificatePrinterPerson(out unsuccessfulResult);
            if (certificatePrinterPerson == null)
                return unsuccessfulResult;

            Context.Initialize(workingDirectoryPath, year, quarter, eiaChairPerson, hrPeople, luncheonPlannerPeople,
                certificatePrinterPerson);

            _configurationStorage.SaveConfiguration();

            return CommandOutput.None.Success;
        }

        private Person GetEiaChairPerson(out CommandResult<CommandOutput.None> unsuccessfulResult)
        {
            var currentEiaChairPerson = Context.IsInitialized ? Context.EiaChairPerson : null;
            return GetPerson(@"EIA Chairperson", currentEiaChairPerson, out unsuccessfulResult);
        }

        private List<Person> GetHrPeople(out CommandResult<CommandOutput.None> unsuccessfulResult)
        {
            var currentHrPeople = Context.IsInitialized ? Context.HrPeople : new List<Person> {null, null};
            var hrPeople = GetPeople(@"Human Resources person #{0}", currentHrPeople, out unsuccessfulResult);
            return hrPeople;
        }

        private List<Person> GetLuncheonPlannerPeople(out CommandResult<CommandOutput.None> unsuccessfulResult)
        {
            var currentHrPeople = Context.IsInitialized ? Context.LuncheonPlannerPeople : new List<Person> {null, null};
            var hrPeople = GetPeople(@"luncheon planner #{0}", currentHrPeople, out unsuccessfulResult);
            return hrPeople;
        }

        private Person GetCertificatePrinterPerson(out CommandResult<CommandOutput.None> unsuccessfulResult)
        {
            var currentCertificatePrinterPerson = Context.IsInitialized ? Context.CertificatePrinterPerson : null;
            return GetPerson(@"award certificate printer", currentCertificatePrinterPerson, out unsuccessfulResult);
        }

        private List<Person> GetPeople(string personTitleFormat, IReadOnlyList<Person> currentPeople,
            out CommandResult<CommandOutput.None> unsuccessfulResult)
        {
            var people = new List<Person>();

            for (var i = 0; i < currentPeople.Count; ++i)
            {
                var id = i + 1;
                var currentPerson = currentPeople[i];

                var person = GetPerson(string.Format(personTitleFormat, id), currentPerson, out unsuccessfulResult);
                if (person == null)
                    return null;

                people.Add(person);
            }

            unsuccessfulResult = null;
            return people;
        }

        private Person GetPerson(string personTitle, Person currentPerson,
            out CommandResult<CommandOutput.None> unsuccessfulResult)
        {
            var personNameCommandInput = new GetPersonNameCommand.Input(personTitle, currentPerson?.Name);
            var personName = GetCommandResult(new GetPersonNameCommand(Context, _globalAddressList),
                personNameCommandInput, out unsuccessfulResult);

            if (personName == null)
                return null;

            var getPersonEmailAddressCommandInput =
                new GetPersonEmailAddressCommand.Input(personName, currentPerson?.EmailAddress);
            var emailAddress = GetCommandResult(new GetPersonEmailAddressCommand(Context, _globalAddressList),
                getPersonEmailAddressCommandInput, out unsuccessfulResult);

            return emailAddress == null ? null : Person.Create(personName, OfficeLocation.EiaTeamMember, emailAddress);
        }

        private static TValue GetCommandResult<TValue>(InitializeCommandBase<TValue> command,
            out CommandResult<CommandOutput.None> unsuccessfulResult)
            where TValue : class
        {
            return GetCommandResult(command, CommandInput.None.Instance,
                out unsuccessfulResult);
        }

        private static TValue GetCommandResult<TCommandInput, TValue>(
            InitializeCommandBase<TCommandInput, TValue> command,
            TCommandInput commandInput, out CommandResult<CommandOutput.None> unsuccessfulResult)
            where TCommandInput : CommandInput
            where TValue : class
        {
            var commandResult = command.Run(commandInput);

            switch (commandResult.ResultType)
            {
                default:
                case CommandResultType.Abort:
                    unsuccessfulResult = CommandOutput.None.Abort;
                    return null;
                case CommandResultType.Failure:
                    unsuccessfulResult = CommandOutput.None.Failure(commandResult.Exception);
                    return null;
                case CommandResultType.Success:
                    unsuccessfulResult = null;
                    return commandResult.Output.Value;
            }
        }
    }
}