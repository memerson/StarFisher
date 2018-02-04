using System;
using System.Collections.Generic;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Exit;
using StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses;
using StarFisher.Console.Menu.FixNomineeWriteUps;
using StarFisher.Console.Menu.TopLevelMenu;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
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
            var filePath =
                FilePath.Create(
                    @"C:\Users\memerson\Desktop\EIA\2018\Q1\SurveyMonkeyExport\Data_All_180109\Excel\Star Awards for Quarterly Peer Recognition.xlsx",
                    true);
            var configuration = new StarFisherConfiguration();
            var nominationListRepository = new NominationListRepository();
            var excelFileFactory = new ExcelFileFactory();
            var mailMergeFactory = new MailMergeFactory(excelFileFactory);
            var emailFactory = new EmailFactory(configuration);
            var nominationList = nominationListRepository.LoadSurveyExport(filePath, Quarter.Fourth, Year.Create(2017));
            var globalAddressList = new GlobalAddressList();
            StarFisherContext.Current.SetContextNominationList(nominationList);

            PrintSplash();

            var awardWinnerList = new AwardWinnerList(Quarter.Fourth, Year.Create(2017));
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
            //    var nominations = nominationList.Nominations
            //        .Where(n => n.NomineeName == starValuesWinnerName)
            //        .ToList();

            //    // TODO: Handle same name different office

            //    var person = nominations.Select(n => n.Nominee).First();
            //    var writeUps = nominations.Select(n => n.WriteUp).ToList();
            //    var companyValues = nominations.SelectMany(n => n.CompanyValues).Distinct().OrderBy(cv => cv.Value).ToList();
            //    awardWinnerList.AddStarValuesWinner(person, companyValues, writeUps);
            //}

            var menuItems = new List<MenuItem>
            {
                new FixNomineeNamesAndEmailAddressesMenuItemCommand().GetMenuItem(new FixNomineeNamesAndEmailAddressesMenuItemCommand.Input(globalAddressList, nominationList)),
                new FixNomineeWriteUpsMenuItemCommand().GetMenuItem(new FixNomineeWriteUpsMenuItemCommand.Input(nominationList)),
                new ExitCommand().GetMenuItem(CommandInput.None.Instance)
            };

            var topLevelMenu = new TopLevelMenuCommand().GetMenuItem(new TopLevelMenuCommand.Input(menuItems));
            topLevelMenu.Run();
        }

        //private static bool HandleCommand(IEnumerable<ICommand> commands)
        //{
        //    var commandText = System.Console.ReadLine();
        //    if (string.IsNullOrWhiteSpace(commandText))
        //        return true;

        //    foreach (var command in commands)
        //    {
        //        var result = command.TryExecute(commandText);

        //        if (!result.ExecutionAttempted)
        //            continue;

        //        if (command is ExitCommand)
        //            return false;

        //        if (result.ExecutionFailed)
        //        {
        //            System.Console.WriteLine(result.ErrorText ?? "Something strange happened.");
        //            break;
        //        }

        //        if (result.ExecutionSucceeded)
        //            break;
        //    }

        //    return true;
        //}

        private static void PrintSplash()
        {
            System.Console.WriteLine();
            System.Console.WriteLine(@"        .");
            System.Console.WriteLine(@"       ,O,");
            System.Console.WriteLine(@"      ,OOO,");
            System.Console.WriteLine(@"'oooooOOOOOooooo'     _________ __              ___________.__       .__");
            System.Console.WriteLine(@"  `OOOOOOOOOOO`      /   _____//  |______ ______\_   _____/|__| _____|  |__   ___________  ");
            System.Console.WriteLine(@"    `OOOOOOO`        \_____  \\   __\__  \\_  __ \    __)  |  |/  ___/  |  \_/ __ \_  __ \");
            System.Console.WriteLine(@"    OOOO'OOOO        /        \|  |  / __ \|  | \/     \   |  |\___ \|   Y  \  ___/|  | \/");
            System.Console.WriteLine(@"   OOO'   'OOO      /_______  /|__| (____  /__|  \___  /   |__/____  >___|  /\___  >__|    ");
            System.Console.WriteLine(@"  O'         'O             \/           \/          \/            \/     \/     \/        ");
            System.Console.WriteLine();
            System.Console.WriteLine();
        }
    }
}