using System.Collections.Generic;
using StarFisher.Console.Commands;
using StarFisher.Console.Commands.Common;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;
using StarFisher.Office.Outlook;
using StarFisher.Office.Word;
using StarFisher.Office.Word.MailMergeTemplates;

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