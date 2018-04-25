using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Office.Outlook;

namespace StarFisher.Console.Menu.CreateLuncheonInviteeListEmail
{
    internal class CreateLuncheonInviteeListEmailMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Create luncheon invitee list email";

        private const string SuccessMessage =
            @"Success! You should now see an email ready to review and send on to the luncheon planner(s).";

        private readonly IEmailFactory _emailFactory;

        public CreateLuncheonInviteeListEmailMenuItemCommand(IStarFisherContext context, IEmailFactory emailFactory)
            : base(context, CommandTitle, SuccessMessage)
        {
            _emailFactory = emailFactory ?? throw new ArgumentNullException(nameof(emailFactory));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;
            using (var email = _emailFactory.GetLuncheonInviteeListEmail(nominationList))
            {
                email.Display();
            }

            return CommandOutput.None.Success;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized
                   && Context.NominationListContext.HasNominationListLoaded
                   && Context.NominationListContext.NominationList.HasNominations
                   && Context.NominationListContext.NominationList.HasAwardsLuncheonInvitees;
        }
    }
}