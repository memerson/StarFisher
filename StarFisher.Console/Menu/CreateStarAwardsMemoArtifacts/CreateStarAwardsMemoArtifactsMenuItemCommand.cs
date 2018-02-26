using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Office.Word;

namespace StarFisher.Console.Menu.CreateStarAwardsMemoArtifacts
{
    public class CreateStarAwardsMemoArtifactsMenuItemCommand : MenuItemCommandBase
    {
        private readonly IStarAwardsMemoHelper _memoHelper;

        private const string CommandTitle =
            @"Create artifacts used to create the Star Awards memo for the Communications team";

        public CreateStarAwardsMemoArtifactsMenuItemCommand(IStarFisherContext context,
            IStarAwardsMemoHelper memoHelper)
            : base(context, CommandTitle, GetSuccessMessage(context))
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

        private static string GetSuccessMessage(IConfiguration context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return $@"Success! You can find the items used to create the Star Awards memo saved in your working directory ({context.WorkingDirectoryPath.Value}).";
        }
    }
}
