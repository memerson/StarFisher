using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Context;
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

            var workingDirectoryPath = DirectoryPath.Create(@"C:\Users\memerson\Desktop\EIA\StarFisher");
            var year = Year.Create(2017);
            var quarter = Quarter.Fourth;
            var configuration = new StarFisherConfiguration();
            var nominationListRepository = new NominationListRepository(workingDirectoryPath);
            var awardWinnerListRepository = new AwardWinnerListRepository(workingDirectoryPath);
            var excelFileFactory = new ExcelFileFactory();
            var mailMergeFactory = new MailMergeFactory(excelFileFactory);
            var emailFactory = new EmailFactory(configuration);
            var globalAddressList = new GlobalAddressList();
            StarFisherContext.Initialize(nominationListRepository, awardWinnerListRepository, year, quarter);
            StarFisherContext.Current.NominationListContext.LoadSurveyExport(filePath);

            PrintSplash();

            var awardWinnerList = new AwardWinnerList(year, quarter);
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

            foreach (var starValuesWinnerName in starValuesWinnerNames.OrderBy(n => n.FullNameLastNameFirst))
            {
                var nominations = StarFisherContext.Current.NominationListContext.NominationList.Nominations
                    .Where(n => n.NomineeName == starValuesWinnerName)
                    .ToList();

                // TODO: Handle same name different office

                var person = nominations.Select(n => n.Nominee).First();
                var writeUps = nominations.Select(n => n.WriteUp).ToList();
                var companyValues = nominations.SelectMany(n => n.CompanyValues).Distinct().OrderBy(cv => cv.Value).ToList();
                awardWinnerList.AddStarValuesWinner(person, companyValues, writeUps);
            }

            var menuItemCommandss = new List<IMenuItemCommand>
            {
                new FixNomineeNamesAndEmailAddressesMenuItemCommand(globalAddressList),
                new FixNomineeWriteUpsMenuItemCommand(),
                new ExitCommand()
            };

            var topLevelMenu = new TopLevelMenuCommand(menuItemCommandss);
            topLevelMenu.Run();
        }

        private static void PrintSplash()
        {
            System.Console.WriteLine();
            System.Console.WriteLine(@"        .");
            System.Console.WriteLine(@"       ,O,");
            System.Console.WriteLine(@"      ,OOO,");
            System.Console.WriteLine(@"'oooooOOOOOooooo'     _________ __              ___________.__       .__");
            System.Console.WriteLine(@"  `OOOOOOOOOOO`      /   _____//  |______ ______\_   _____/|__| _____|  |__   ___________  ");
            System.Console.WriteLine(@"    `OOOOOOO`        \_____  \\   __\__  \\_  __ \    __)  |  |/  ___/  |  \_/ __ \_  __ \ ");
            System.Console.WriteLine(@"    OOOO'OOOO        /        \|  |  / __ \|  | \/     \   |  |\___ \|   Y  \  ___/|  | \/ ");
            System.Console.WriteLine(@"   OOO'   'OOO      /_______  /|__| (____  /__|  \___  /   |__/____  >___|  /\___  >__|    ");
            System.Console.WriteLine(@"  O'         'O             \/           \/          \/            \/     \/     \/        ");
            System.Console.WriteLine(@"                                                                         Nashville Edition ");
            System.Console.WriteLine(@"                                                                         By Matt Emerson   ");
            System.Console.WriteLine();
            System.Console.WriteLine();
        }
    }
}