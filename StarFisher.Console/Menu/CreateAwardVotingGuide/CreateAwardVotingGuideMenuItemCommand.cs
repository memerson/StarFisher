using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Word;

namespace StarFisher.Console.Menu.CreateAwardVotingGuide
{
    public class CreateAwardVotingGuideMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Create Star Awards voting guide(s)";
        private readonly IMailMergeFactory _mailMergeFactory;

        public CreateAwardVotingGuideMenuItemCommand(IStarFisherContext context, IMailMergeFactory mailMergeFactory)
            : base(context, CommandTitle, GetSuccessMessage(context))
        {
            _mailMergeFactory = mailMergeFactory ?? throw new ArgumentNullException(nameof(mailMergeFactory));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;

            foreach (var awardType in AwardType.ValidAwardTypes)
            {
                if (!nominationList.HasNominationsForAward(awardType))
                    continue;

                var fileName = awardType.GetVotingGuideFileName(Context.AwardsPeriod);
                var filePath = Context.WorkingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);
                var mailMerge = _mailMergeFactory.GetVotingGuideMailMerge(awardType, nominationList);
                mailMerge.Execute(filePath);
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

            return $@"Success! You can find the voting guide document(s) saved in your working directory ({
                    context.WorkingDirectoryPath.Value
                }).";
        }
    }
}