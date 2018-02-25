using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.Faults;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Word;

namespace StarFisher.Console.Menu.CreateAwardVotingGuide
{
    public class CreateAwardVotingGuideMenuItemCommand : MenuItemCommandBase
    {
        private readonly AwardType _awardType;
        private readonly IMailMergeFactory _mailMergeFactory;

        public CreateAwardVotingGuideMenuItemCommand(IStarFisherContext context, IMailMergeFactory mailMergeFactory,
            AwardType awardType)
            : base(context, GetCommandTitle(awardType))
        {
            _mailMergeFactory = mailMergeFactory ?? throw new ArgumentNullException(nameof(mailMergeFactory));
            _awardType = awardType ?? throw new ArgumentNullException(nameof(awardType));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;
            var fileName = GetVotingKeyFileName();

            var filePath = Context.WorkingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);
            var mailMerge = _mailMergeFactory.GetVotingGuideMailMerge(_awardType, nominationList);
            mailMerge.Execute(filePath);

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

            return $@"Create {awardType.PrettyName} voting guide";
        }

        private static string GetSuccessMessage(IConfiguration context, AwardType awardType)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            return $@"Success! You can find {
                    GetVotingGuideFileName(context, awardType)
                } saved in your working directory ({context.WorkingDirectoryPath.Value}).";
        }

        private string GetVotingKeyFileName()
        {
            return GetVotingGuideFileName(Context, _awardType);
        }

        private static string GetVotingGuideFileName(IConfiguration context, AwardType awardType)
        {
            return awardType.GetVotingGuideFileName(context.Year, context.Quarter);
        }
    }
}