//using System;
//using System.Collections.Generic;
//using System.Linq;
//using StarFisher.Console.Menu.Common;
//using StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Commands;
//using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
//using StarFisher.Domain.ValueObjects;
//using StarFisher.Office.Outlook.AddressBook;
//using NomineeNameChanges = System.Collections.Generic.IReadOnlyCollection<StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Commands.FixNomineeNamesCommand.Output.NomineeNameChange>;

//namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses
//{
//    class FixNomineeNamesAndEmailAddressesWorkflow : ICommand<CommandInput.None, CommandOutput.None>
//    {
//        private readonly IGlobalAddressList _globalAddressList;
//        private readonly NominationList _nominationList;

//        public FixNomineeNamesAndEmailAddressesWorkflow(IGlobalAddressList globalAddressList, NominationList nominationList)
//        {
//            _globalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
//            _nominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
//        }

//        public string Title => @"Fix nominee names and email addresses";

//        public CommandResult<CommandOutput.None> Run(CommandInput.None inputx)
//        {
//            var allNomineeNames = _nominationList.Nominations.Select(n => n.NomineeName).Distinct().ToList();

//            var nomineeEmailAddressesByName = _nominationList.Nominations
//                .Select(n => new { n.NomineeName, n.NomineeEmailAddress })
//                .GroupBy(n => n.NomineeName, n => n.NomineeEmailAddress)
//                .ToDictionary(g => g.Key, g => g.ToList());

//            GetUnrecognizedNomineeNamesAndEmailAddresses(nomineeEmailAddressesByName,
//                out List<PersonName> unrecognizedNames,
//                out HashSet<EmailAddress> unrecognizedEmailAddresses);

//            var commandResult = FixNomineeNames(allNomineeNames, unrecognizedNames, out NomineeNameChanges nomineeNameChanges);

//            if (commandResult.ResultType != CommandResultType.Success)
//                return commandResult;

//            foreach (var nomineeNameChange in nomineeNameChanges)
//            {
//                var oldNomineeName = nomineeNameChange.OldNomineeName;
//                var newNomineeName = nomineeNameChange.NewNomineeName;

//                if (!nomineeEmailAddressesByName.TryGetValue(oldNomineeName, out List<EmailAddress> emailAddresses))
//                    continue;

//                foreach (var emailAddress in emailAddresses)
//                {
//                    bool = deriveEmailAddress
//                    if (unrecognizedEmailAddresses.Contains(emailAddress))
//                    {
//                        var newDefaultEmailAddress = newNomineeName.DerivedEmailAddress;
//                        var queryResult = _globalAddressList.QueryNominee(newNomineeName, newDefaultEmailAddress);

//                        var deriveEmailAddress = queryResult == PersonQueryResult.NameAndEmailAddressFound;
//                    }
//                }
//            }
//        }

//        private static CommandResult<CommandOutput.None> FixNomineeNames(IReadOnlyCollection<PersonName> allNomineeNames, IReadOnlyCollection<PersonName> unrecognizedNames, out NomineeNameChanges nomineeNameChanges)
//        {
//            nomineeNameChanges = null;

//            var fixNomineeNamesCommand = new FixNomineeNamesCommand();
//            var input = new FixNomineeNamesCommand.Input(allNomineeNames, unrecognizedNames);
//            var commandResult = fixNomineeNamesCommand.Run(input);

//            switch (commandResult.ResultType)
//            {
//                case CommandResultType.Abort:
//                    return CommandResult<CommandOutput.None>.Abort();
//                case CommandResultType.Failure:
//                    return CommandResult<CommandOutput.None>.Failure(commandResult.Exception);
//                case CommandResultType.Success:
//                default:
//                    nomineeNameChanges = commandResult.Output.NomineeNameChanges;
//                    return CommandResult<CommandOutput.None>.Success(CommandOutput.None.Instance);
//            }
//        }

//        private void GetUnrecognizedNomineeNamesAndEmailAddresses(Dictionary<PersonName, List<EmailAddress>> nomineeEmailAddressByName, out List<PersonName> unrecognizedNames, out HashSet<EmailAddress> unrecognizedEmailAddresses)
//        {
//            unrecognizedNames = new List<PersonName>();
//            unrecognizedEmailAddresses = new HashSet<EmailAddress>();

//            foreach (var kvp in nomineeEmailAddressByName)
//            {
//                var nomineeName = kvp.Key;

//                foreach (var emailAddress in kvp.Value)
//                {
//                    var queryResult = _globalAddressList.QueryNominee(nomineeName, emailAddress);

//                    switch (queryResult)
//                    {
//                        case PersonQueryResult.NameNotFound:
//                        case PersonQueryResult.MultipleNameMatchesFound:
//                            unrecognizedNames.Add(nomineeName);
//                            break;

//                        case PersonQueryResult.EmailAddressDoesNotMatch:
//                            unrecognizedEmailAddresses.Add(emailAddress);
//                            break;
//                    }
//                }
//            }
//        }
//    }
//}
