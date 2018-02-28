using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.CreateVotingSurveyReviewEmail.Parameters;
using StarFisher.Office.Outlook;

namespace StarFisher.Console.Menu.CreateVotingSurveyReviewEmail
{
    public class CreateVotingSurveyReviewEmailMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Create voting survey review email";

        private const string SuccessMessage =
            @"Success! You should now see an email ready to review and send on to the EIA Chairperson(s).";

        private readonly IEmailFactory _emailFactory;

        public CreateVotingSurveyReviewEmailMenuItemCommand(IStarFisherContext context, IEmailFactory emailFactory)
            : base(context, CommandTitle, SuccessMessage)
        {
            _emailFactory = emailFactory ?? throw new ArgumentNullException(nameof(emailFactory));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            if (!TryGetArgumentValue(new VotingSurveyWebLinkParameter(), out string votingSurveyWebLink))
                return CommandOutput.None.Abort;

            var nominationList = Context.NominationListContext.NominationList;
            using (var email = _emailFactory.GetVotingSurveyReviewEmail(nominationList, votingSurveyWebLink))
            {
                email.Display();
            }

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