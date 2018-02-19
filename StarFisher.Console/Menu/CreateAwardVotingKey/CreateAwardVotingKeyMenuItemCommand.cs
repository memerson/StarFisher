using System;
using StarFisher.Console.Context;
using StarFisher.Console.Faults;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Console.Menu.CreateAwardVotingKey
{
    public class CreateAwardVotingKeyMenuItemCommand : MenuItemCommandBase
    {
        private readonly AwardType _awardType;
        private readonly IExcelFileFactory _excelFileFactory;

        public CreateAwardVotingKeyMenuItemCommand(IStarFisherContext context, IExcelFileFactory excelFileFactory,
            AwardType awardType)
            : base(context, GetCommandTitle(awardType), GetSuccessMessage(context, awardType))
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _awardType = awardType ?? throw new ArgumentNullException(nameof(awardType));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;
            var fileName = GetVotingKeyFileName();

            if (!Context.WorkingDirectoryPath.TryGetFilePathForFileInDirectory(fileName, false, false,
                out FilePath filePath))
                throw InvalidFilePathException.WorkingDirectoryFilePathInvalid(fileName);

            using (var excelFile = _excelFileFactory.GetVotingKeyExcelFile(_awardType, nominationList))
            {
                excelFile.Save(filePath);
            }

            return CommandOutput.None.Success;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized
                   && Context.NominationListContext.HasNominationListLoaded
                   && Context.NominationListContext.NominationList.HasNominationsForAward(_awardType);
        }

        private static string GetCommandTitle(AwardType awardType)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            return $@"Create {awardType.PrettyName} voting key";
        }

        private static string GetSuccessMessage(IConfiguration context, AwardType awardType)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            return $@"Success! You can find {
                    GetVotingKeyFileName(context, awardType)
                } saved in your working directory ({context.WorkingDirectoryPath.Value}).";
        }

        private string GetVotingKeyFileName()
        {
            return GetVotingKeyFileName(Context, _awardType);
        }

        private static string GetVotingKeyFileName(IConfiguration context, AwardType awardType)
        {
            return awardType.GetVotingKeyFileName(context.Year, context.Quarter);
        }
    }
}