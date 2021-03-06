﻿using System;
using System.Collections.Generic;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.CreateAwardVotingGuide;
using StarFisher.Console.Menu.CreateAwardVotingKey;
using StarFisher.Console.Menu.CreateCertificateEmail;
using StarFisher.Console.Menu.CreateHumanResourceNomineeValidationEmail;
using StarFisher.Console.Menu.CreateLuncheonInviteeListEmail;
using StarFisher.Console.Menu.CreateStarAwardsMemoArtifacts;
using StarFisher.Console.Menu.CreateVotingKeyEmail;
using StarFisher.Console.Menu.CreateVotingSurveyEmails;
using StarFisher.Console.Menu.DisqualifyNominees;
using StarFisher.Console.Menu.Exit;
using StarFisher.Console.Menu.FixNominees;
using StarFisher.Console.Menu.FixNomineeWriteUps;
using StarFisher.Console.Menu.Initialize;
using StarFisher.Console.Menu.LoadNominationsFromSnapshot;
using StarFisher.Console.Menu.LoadNominationsFromSurveyExport;
using StarFisher.Console.Menu.RemoveNominations;
using StarFisher.Console.Menu.SelectAwardWinner;
using StarFisher.Console.Menu.SendNominationNotificationEmails;
using StarFisher.Console.Menu.TopLevelMenu;
using StarFisher.Console.Menu.UnselectAwardWinner;
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
            /* TODO:
             * 1. Changes from PX and Verity fallout
             */

            var excelFileFactory = new ExcelFileFactory();
            var mailMergeFactory = new MailMergeFactory(excelFileFactory);
            var globalAddressList = GetGlobalAddressList();
            var configurationStorage = new ConfigurationStorage();

            InitializeApplication(configurationStorage, globalAddressList);

            var emailFactory = new EmailFactory(StarFisherContext.Instance, excelFileFactory, mailMergeFactory);
            var memoHelper = new StarAwardsMemoHelper(excelFileFactory, mailMergeFactory);

            var menuItemCommands = new List<IMenuItemCommand>
            {
                new LoadNominationsFromSnapshotMenuItemCommand(StarFisherContext.Instance),
                new LoadNominationsFromSurveyExportMenuItemCommand(StarFisherContext.Instance),
                new FixNomineesMenuItemCommand(StarFisherContext.Instance, globalAddressList),
                new FixNomineeWriteUpsMenuItemCommand(StarFisherContext.Instance),
                new DisqualifyNomineesMenuItemCommand(StarFisherContext.Instance),
                new RemoveNominationMenuItemCommand(StarFisherContext.Instance),
                new CreateHumanResourceNomineeValidationEmailMenuItemCommand(StarFisherContext.Instance, emailFactory),
                new CreateAwardVotingKeyMenuItemCommand(StarFisherContext.Instance, excelFileFactory),
                new CreateAwardVotingGuideMenuItemCommand(StarFisherContext.Instance, mailMergeFactory),
                new CreateVotingSurveyEmailsMenuItemCommand(StarFisherContext.Instance, emailFactory),
                new CreateVotingKeyEmailMenuItemCommand(StarFisherContext.Instance, emailFactory),
                new CreateLuncheonInviteeListEmailMenuItemCommand(StarFisherContext.Instance, emailFactory),
                new SendNominationNotificationEmailsMenuItemCommand(StarFisherContext.Instance, mailMergeFactory),
                new SelectAwardWinnerMenuItemCommand(StarFisherContext.Instance),
                new UnselectAwardWinnerMenuItemCommand(StarFisherContext.Instance),
                new CreateCertificateEmailMenuItemCommand(StarFisherContext.Instance, emailFactory),
                new CreateStarAwardsMemoArtifactsMenuItemCommand(StarFisherContext.Instance, memoHelper),
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
                StarFisherConsole.Instance.WriteLine();
                StarFisherConsole.Instance.WriteLine(@"You must complete StarFisher's initial set-up to continue.");
                StarFisherConsole.Instance.WriteLine();

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

            StarFisherConsole.Instance.WriteLine();
            StarFisherConsole.Instance.WriteLine(
                @"Failed to load saved configuration. You will need to re-run the initialization workflow.");
            StarFisherConsole.Instance.WriteLine();
            StarFisherConsole.Instance.PrintException(exception);

            return false;
        }
    }
}