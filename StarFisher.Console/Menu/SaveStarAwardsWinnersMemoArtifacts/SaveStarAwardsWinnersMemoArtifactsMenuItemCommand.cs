using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Office.Word;

namespace StarFisher.Console.Menu.SaveStarAwardsWinnersMemoArtifacts
{
    public class SaveStarAwardsWinnersMemoArtifactsMenuItemCommand : MenuItemCommandBase
    {
        private readonly IStarAwardsMemoHelper _memoHelper;

        private const string CommandTitle =
            @"Save artifacts used to create the Star Awards memo for the Communications team";

        private const string SuccessMessage =
            @"Success! Your working directory now contains all relevant artifacts for creating the memo.";

        public SaveStarAwardsWinnersMemoArtifactsMenuItemCommand(IStarFisherContext context,
            IStarAwardsMemoHelper memoHelper)
            : base(context, CommandTitle, SuccessMessage)
        {
            _memoHelper = memoHelper ?? throw new ArgumentNullException(nameof(memoHelper));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;
            var workingDirectoryPath = Context.WorkingDirectoryPath;

            _memoHelper.SaveArtifacts(workingDirectoryPath, nominationList);

            return CommandOutput.None.Success;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized
                   && Context.NominationListContext.HasNominationListLoaded
                   && Context.NominationListContext.NominationList.HasAwardWinners;
        }
    }
}
