using System.Collections.Generic;
using StarFisher.Console.Commands;
using StarFisher.Console.Commands.Common;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;
using StarFisher.Office.Outlook;
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
            StarFisherContext.Current.SetContextNominationList(nominationList);

            var winners = new AwardWinnerList(Quarter.Fourth, Year.Create(2017));
            var winnerName = PersonName.Create("Jane Doe");
            winners.AddStarValuesWinner(winnerName, OfficeLocation.EchoBrentwood,
                new[] {CompanyValue.CustomerFocus, CompanyValue.Innovation},
                new[]
                {
                    NominationWriteUp.Create(winnerName, "Lorem ipsum dolor sit amet, ex mea nibh populo, mei doming eligendi sententiae no. An eam vitae ceteros constituam, at vim essent iudicabit intellegam. Ad assum aperiri nec, ex oportere adolescens has. Vim eu mazim utinam viderer. Modo justo at his, cum aperiam fuisset an, ex falli animal eos. Utinam legere ei ius, melius signiferumque qui te, ad quidam eripuit his. Vim nulla tritani accusam an, vel ut aliquam minimum eleifend. Saepe habemus necessitatibus his at, pri probo dignissim et. Sea sint nominavi consulatu eu, eam periculis dignissim in. Ad pro inani volumus adolescens."),
                        NominationWriteUp.Create(winnerName, "Lorem ipsum dolor sit amet, ex mea nibh populo, mei doming eligendi sententiae no. An eam vitae ceteros constituam, at vim essent iudicabit intellegam. Ad assum aperiri nec, ex oportere adolescens has. Vim eu mazim utinam viderer. Modo justo at his, cum aperiam fuisset an, ex falli animal eos. Utinam legere ei ius, melius signiferumque qui te, ad quidam eripuit his. Vim nulla tritani accusam an, vel ut aliquam minimum eleifend. Saepe habemus necessitatibus his at, pri probo dignissim et. Sea sint nominavi consulatu eu, eam periculis dignissim in. Ad pro inani volumus adolescens.")
                }, EmailAddress.Create("jonh.doe@healthstream.com"));
            winners.AddStarValuesWinner(winnerName, OfficeLocation.EchoBrentwood,
                new[] { CompanyValue.CustomerFocus, CompanyValue.Innovation },
                new[]
                {
                    NominationWriteUp.Create(winnerName, "Lorem ipsum dolor sit amet, ex mea nibh populo, mei doming eligendi sententiae no. An eam vitae ceteros constituam, at vim essent iudicabit intellegam. Ad assum aperiri nec, ex oportere adolescens has. Vim eu mazim utinam viderer. Modo justo at his, cum aperiam fuisset an, ex falli animal eos. Utinam legere ei ius, melius signiferumque qui te, ad quidam eripuit his. Vim nulla tritani accusam an, vel ut aliquam minimum eleifend. Saepe habemus necessitatibus his at, pri probo dignissim et. Sea sint nominavi consulatu eu, eam periculis dignissim in. Ad pro inani volumus adolescens."),
                    NominationWriteUp.Create(winnerName, "Lorem ipsum dolor sit amet, ex mea nibh populo, mei doming eligendi sententiae no. An eam vitae ceteros constituam, at vim essent iudicabit intellegam. Ad assum aperiri nec, ex oportere adolescens has. Vim eu mazim utinam viderer. Modo justo at his, cum aperiam fuisset an, ex falli animal eos. Utinam legere ei ius, melius signiferumque qui te, ad quidam eripuit his. Vim nulla tritani accusam an, vel ut aliquam minimum eleifend. Saepe habemus necessitatibus his at, pri probo dignissim et. Sea sint nominavi consulatu eu, eam periculis dignissim in. Ad pro inani volumus adolescens.")
                }, EmailAddress.Create("jonh.doe@healthstream.com"));

            mailMergeFactory.GetStarValuesWinnersMemoMailMerge(winners).Execute();
            //var outFilePath = @"C:\Users\memerson\Desktop\EIA\2018\Q1\out.xlsx";
            //using (var excelFile = excelFileFactory.GetStarValuesWinnersMemoSourceExcelFile(winners))
            //    excelFile.Save(FilePath.Create(outFilePath, false));

            PrintSplash();

            var commands = new List<ICommand>
            {
                new LoadSurveyExportCommand(nominationListRepository),
                new ListNomineeNamesCommand(),
                new FindInvalidNominationWriteUpsCommand(),
                new ComposeNomineeValidationEmailForHumanResourcesCommand(emailFactory),
                new CreateStarValuesVotingGuideCommand(mailMergeFactory),
                new ExitCommand()
            };

            for (;;)
            {
                if (!HandleCommand(commands))
                    return;
            }
        }

        private static bool HandleCommand(IEnumerable<ICommand> commands)
        {
            var commandText = System.Console.ReadLine();
            if (string.IsNullOrWhiteSpace(commandText))
                return true;

            foreach (var command in commands)
            {
                var result = command.TryExecute(commandText);

                if (!result.ExecutionAttempted)
                    continue;

                if (command is ExitCommand)
                    return false;

                if (result.ExecutionFailed)
                {
                    System.Console.WriteLine(result.ErrorText ?? "Something strange happened.");
                    break;
                }

                if (result.ExecutionSucceeded)
                    break;
            }

            return true;
        }

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