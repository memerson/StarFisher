using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Console.Menu.CreateAwardVotingKey
{
    public class CreateAwardVotingKeyMenuItemCommand : MenuItemCommandBase
    {
        private readonly IExcelFileFactory _excelFileFactory;

        private const string CommandTitle = @"Create Star Awards voting key(s)";

        public CreateAwardVotingKeyMenuItemCommand(IStarFisherContext context, IExcelFileFactory excelFileFactory)
            : base(context, CommandTitle, GetSuccessMessage(context))
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;

            foreach (var awardType in AwardType.ValidAwardTypes)
            {
                if (!nominationList.HasNominationsForAward(awardType))
                    continue;

                var fileName = awardType.GetVotingKeyFileName(Context.AwardsPeriod);

                var filePath = Context.WorkingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);
                using (var excelFile = _excelFileFactory.GetVotingKeyExcelFile(awardType, nominationList))
                {
                    excelFile.Save(filePath);
                }
            }

            return CommandOutput.None.Success;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized
                   && Context.NominationListContext.HasNominationListLoaded
                   && Context.NominationListContext.NominationList.HasNominations;
        }

        private static string GetSuccessMessage(IConfiguration context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return $@"Success! You can find the voting key spreadsheets saved in your working directory ({context.WorkingDirectoryPath.Value}).";
        }
    }
}