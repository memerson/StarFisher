using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.CreateVotingSurveyEmails.Parameters;
using StarFisher.Office.Outlook;

namespace StarFisher.Console.Menu.CreateVotingSurveyEmails
{
    public class CreateVotingSurveyEmailsMenuItemCommand : MenuItemCommandBase
    {
        private readonly IEmailFactory _emailFactory;

        private const string CommandTitle = @"Create voting survey emails";

        private const string SuccessMessage =
            @"Success! You should now see two emails ready to review and send on: one to the EIA Chairperson(s) and one to the EIA team.";

        public CreateVotingSurveyEmailsMenuItemCommand(IStarFisherContext context, IEmailFactory emailFactory) : base(context, CommandTitle, SuccessMessage)
        {
            _emailFactory = emailFactory ?? throw new ArgumentNullException(nameof(emailFactory));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            if (!TryGetArgumentValue(new VotingSurveyWebLinkParameter(), out string votingSurveyWebLink))
                return CommandOutput.None.Abort;

            if(!TryGetArgumentValue(new VotingDeadlineParameter(), out DateTime votingDeadline))
                return CommandOutput.None.Abort;

            if(!TryGetArgumentValue(new ContinueParameter(), out bool doContinue) || !doContinue)
                return CommandOutput.None.Abort;

            var nominationList = Context.NominationListContext.NominationList;

            using (var email = _emailFactory.GetVotingCallToActionEmail(nominationList, votingSurveyWebLink, votingDeadline))
                email.Display();

            using (var email = _emailFactory.GetVotingSurveyReviewEmail(nominationList, votingSurveyWebLink))
                email.Display();

            return CommandOutput.None.Success;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized
                   && Context.NominationListContext.HasNominationListLoaded
                   && Context.NominationListContext.NominationList.HasNominations;
        }
    }
}
