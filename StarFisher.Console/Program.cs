using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.DisqualifyNominees;
using StarFisher.Console.Menu.Exit;
using StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses;
using StarFisher.Console.Menu.FixNomineeWriteUps;
using StarFisher.Console.Menu.Initialize;
using StarFisher.Console.Menu.LoadNominationsFromSnapshot;
using StarFisher.Console.Menu.LoadNominationsFromSurveyExport;
using StarFisher.Console.Menu.RemoveNominations;
using StarFisher.Console.Menu.TopLevelMenu;
using StarFisher.Console.Menu.ValidateNomineesWithHr;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;
using StarFisher.Office.Outlook;
using StarFisher.Office.Outlook.AddressBook;
using StarFisher.Office.Word;

namespace StarFisher.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var excelFileFactory = new ExcelFileFactory();
            var mailMergeFactory = new MailMergeFactory(excelFileFactory);
            var globalAddressList = GetGlobalAddressList();
            var configurationStorage = new ConfigurationStorage();

            InitializeApplication(configurationStorage, globalAddressList);

            var emailFactory = new EmailFactory(StarFisherContext.Current);

            var awardWinnerList = new AwardWinnerList(StarFisherContext.Current.Year, StarFisherContext.Current.Quarter);
            awardWinnerList.AddStarPerformanceAwardWinner(
                Person.Create(PersonName.Create("Makayla Johnson"),
                    OfficeLocation.HighlandRidge,
                    EmailAddress.None),
                AwardAmount.StarPerformanceFullTimeFirstPlace,
                true);
            awardWinnerList.AddStarPerformanceAwardWinner(
                Person.Create(PersonName.Create("Kay Fortner "),
                    OfficeLocation.HighlandRidge,
                    EmailAddress.None),
                AwardAmount.StarPerformanceFullTimeSecondPlace,
                true);
            awardWinnerList.AddStarPerformanceAwardWinner(
                Person.Create(PersonName.Create("Jan Spaeth"),
                    OfficeLocation.HighlandRidge,
                    EmailAddress.None),
                AwardAmount.StarPerformanceFullTimeThirdPlace,
                true);

            awardWinnerList.AddStarPerformanceAwardWinner(
                Person.Create(PersonName.Create("Angela Mayer"),
                    OfficeLocation.HighlandRidge,
                    EmailAddress.None),
                AwardAmount.StarPerformancePartTimeFirstPlace,
                false);
            awardWinnerList.AddStarPerformanceAwardWinner(
                Person.Create(PersonName.Create("Lamesha Wells"),
                    OfficeLocation.HighlandRidge,
                    EmailAddress.None),
                AwardAmount.StarPerformancePartTimeSecondPlace,
                false);
            awardWinnerList.AddStarPerformanceAwardWinner(
                Person.Create(PersonName.Create("John Boggan"),
                    OfficeLocation.HighlandRidge,
                    EmailAddress.None),
                AwardAmount.StarPerformancePartTimeThirdPlace,
                false);

            awardWinnerList.AddRisingPerformanceAwardWinner(
                Person.Create(PersonName.Create("Cesilia Carlos"),
                    OfficeLocation.HighlandRidge,
                    EmailAddress.None),
                AwardAmount.RisingPerformanceFullTime,
                true);
            awardWinnerList.AddRisingPerformanceAwardWinner(
                Person.Create(PersonName.Create("Bernadine Upson"),
                    OfficeLocation.HighlandRidge,
                    EmailAddress.None),
                AwardAmount.RisingPerformancePartTime,
                false);

            var starValuesWinnerNames = new List<PersonName>
            {
                PersonName.Create("Alexandru Rusu"),
                PersonName.Create("Carol Selawski"),
                PersonName.Create("Deanna Buhl"),
                PersonName.Create("Gregory Savage"),
                PersonName.Create("Kristine Dizon"),
                PersonName.Create("Matt Emerson"),
                PersonName.Create("Van Irwin")
            };

            //foreach (var starValuesWinnerName in starValuesWinnerNames.OrderBy(n => n.FullNameLastNameFirst))
            //{
            //    var nominations = StarFisherContext.Current.NominationListContext.NominationList.Nominations
            //        .Where(n => n.NomineeName == starValuesWinnerName)
            //        .ToList();

            //    // TODO: Handle same name different office

            //    var person = nominations.Select(n => n.Nominee).First(); // TODO: Internalize this so we don't need to expose Nominee property.
            //    var writeUps = nominations.Select(n => n.WriteUp).ToList();
            //    var companyValues = nominations.SelectMany(n => n.CompanyValues).Distinct().OrderBy(cv => cv.Value).ToList();
            //    awardWinnerList.AddStarValuesWinner(person, companyValues, writeUps);
            //}

            var menuItemCommands = new List<IMenuItemCommand>
            {
                new LoadNominationsFromSnapshotMenuItemCommand(StarFisherContext.Current),
                new LoadNominationsFromSurveyExportMenuItemCommand(StarFisherContext.Current),
                new FixNomineeNamesAndEmailAddressesMenuItemCommand(StarFisherContext.Current, globalAddressList),
                new FixNomineeWriteUpsMenuItemCommand(StarFisherContext.Current),
                new DisqualifyNomineesMenuItemCommand(StarFisherContext.Current),
                new RemoveNominationMenuItemCommand(StarFisherContext.Current),
                new ValidateNomineesWithHrMenuItemCommand(StarFisherContext.Current, emailFactory),
                new InitializeApplicationMenuItemCommand(StarFisherContext.Current, globalAddressList, configurationStorage),
                new ExitCommand(StarFisherContext.Current)
            };

            var topLevelMenu = new TopLevelMenuCommand(StarFisherContext.Current, menuItemCommands);
            topLevelMenu.Run();
        }

        private static GlobalAddressList GetGlobalAddressList()
        {
            var globalAddressList = new GlobalAddressList();

            globalAddressList.InitializationStarted += (s, e) =>
            {
                StarFisherConsole.Instance.WriteLine();
                StarFisherConsole.Instance.WriteLineBlue(@"Loading the global address list -- please wait....");
            };

            return globalAddressList;
        }

        private static void InitializeApplication(ConfigurationStorage configurationStorage, IGlobalAddressList globalAddressList)
        {
            if (!TryInitializeContextFromSavedConfiguration(configurationStorage))
            {
                do
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine(@"You must complete StarFisher's initial set-up to continue.");
                    System.Console.WriteLine();

                    var initializationCommand = new InitializeApplicationMenuItemCommand(StarFisherContext.Current, globalAddressList, configurationStorage);
                    initializationCommand.Run();
                } while (!StarFisherContext.Current.IsInitialized);
            }
        }

        private static bool TryInitializeContextFromSavedConfiguration(ConfigurationStorage configurationStorage)
        {
            var initialized = configurationStorage.InitializeFromConfiguration(out Exception exception);

            if (!initialized && exception != null)
            {
                System.Console.WriteLine();
                System.Console.WriteLine(
                    @"Failed to load saved configuration. You will need to re-run the initialization workflow.");
                System.Console.WriteLine(exception.ToString());
                System.Console.WriteLine();
            }

            return initialized;
        }
    }
}