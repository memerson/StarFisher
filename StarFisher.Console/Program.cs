using System;
using System.Collections.Generic;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.CreateAwardVotingGuide;
using StarFisher.Console.Menu.CreateAwardVotingKey;
using StarFisher.Console.Menu.CreateHumanResourceNomineeValidationEmail;
using StarFisher.Console.Menu.CreateLuncheonInviteeListEmail;
using StarFisher.Console.Menu.CreateVotingKeyEmail;
using StarFisher.Console.Menu.CreateVotingSurveyReviewEmail;
using StarFisher.Console.Menu.DisqualifyNominees;
using StarFisher.Console.Menu.Exit;
using StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses;
using StarFisher.Console.Menu.FixNomineeWriteUps;
using StarFisher.Console.Menu.Initialize;
using StarFisher.Console.Menu.LoadNominationsFromSnapshot;
using StarFisher.Console.Menu.LoadNominationsFromSurveyExport;
using StarFisher.Console.Menu.RemoveNominations;
using StarFisher.Console.Menu.SelectAwardWinner;
using StarFisher.Console.Menu.TopLevelMenu;
using StarFisher.Console.Menu.UnselectAwardWinner;
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

            var emailFactory = new EmailFactory(StarFisherContext.Instance, excelFileFactory, mailMergeFactory);

            var menuItemCommands = new List<IMenuItemCommand>
            {
                new LoadNominationsFromSnapshotMenuItemCommand(StarFisherContext.Instance),
                new LoadNominationsFromSurveyExportMenuItemCommand(StarFisherContext.Instance),
                new FixNomineeNamesAndEmailAddressesMenuItemCommand(StarFisherContext.Instance, globalAddressList),
                new FixNomineeWriteUpsMenuItemCommand(StarFisherContext.Instance),
                new DisqualifyNomineesMenuItemCommand(StarFisherContext.Instance),
                new RemoveNominationMenuItemCommand(StarFisherContext.Instance),
                new CreateHumanResourceNomineeValidationEmailMenuItemCommand(StarFisherContext.Instance, emailFactory),
                new CreateAwardVotingKeyMenuItemCommand(StarFisherContext.Instance, excelFileFactory,
                    AwardType.StarValues),
                new CreateAwardVotingKeyMenuItemCommand(StarFisherContext.Instance, excelFileFactory,
                    AwardType.RisingStar),
                new CreateAwardVotingGuideMenuItemCommand(StarFisherContext.Instance, mailMergeFactory,
                    AwardType.StarValues),
                new CreateAwardVotingGuideMenuItemCommand(StarFisherContext.Instance, mailMergeFactory,
                    AwardType.RisingStar),
                new CreateVotingSurveyReviewEmailMenuItemCommand(StarFisherContext.Instance, emailFactory),
                new CreateVotingKeyEmailMenuItemCommand(StarFisherContext.Instance, emailFactory),
                new CreateLuncheonInviteeListEmailMenuItemCommand(StarFisherContext.Instance, emailFactory),
                new SelectAwardWinnerMenuItemCommand(StarFisherContext.Instance),
                new UnselectAwardWinnerMenuItemCommand(StarFisherContext.Instance),
                new InitializeApplicationMenuItemCommand(StarFisherContext.Instance, globalAddressList,
                    configurationStorage),
                new ExitCommand(StarFisherContext.Instance)
            };

            var topLevelMenu = new TopLevelMenuCommand(StarFisherContext.Instance, menuItemCommands);
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

        private static void InitializeApplication(ConfigurationStorage configurationStorage,
            IGlobalAddressList globalAddressList)
        {
            if (TryInitializeContextFromSavedConfiguration(configurationStorage))
                return;

            do
            {
                System.Console.WriteLine();
                System.Console.WriteLine(@"You must complete StarFisher's initial set-up to continue.");
                System.Console.WriteLine();

                var initializationCommand = new InitializeApplicationMenuItemCommand(StarFisherContext.Instance,
                    globalAddressList, configurationStorage);
                initializationCommand.Run();
            } while (!StarFisherContext.Instance.IsInitialized);
        }

        private static bool TryInitializeContextFromSavedConfiguration(ConfigurationStorage configurationStorage)
        {
            var initialized = configurationStorage.InitializeFromConfiguration(out Exception exception);

            if (initialized || exception == null)
                return initialized;

            System.Console.WriteLine();
            System.Console.WriteLine(
                @"Failed to load saved configuration. You will need to re-run the initialization workflow.");
            System.Console.WriteLine(exception.ToString());
            System.Console.WriteLine();

            return false;
        }
    }
}