using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook;

namespace StarFisher.Console.Menu.CreateVotingKeyEmail
{
    public class CreateVotingKeyEmailMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Create voting key email";

        private const string SuccessMessage =
            @"Success! You should now see an email ready to review and send on to the EIA Chairperson(s).";

        private readonly IEmailFactory _emailFactory;

        public CreateVotingKeyEmailMenuItemCommand(IStarFisherContext context, IEmailFactory emailFactory)
            : base(context, CommandTitle, SuccessMessage)
        {
            _emailFactory = emailFactory ?? throw new ArgumentNullException(nameof(emailFactory));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;
            using (var email = _emailFactory.GetVotingKeyEmail(nominationList))
            {
                email.Display();
            }

            return CommandOutput.None.Success;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized
                   && Context.NominationListContext.HasNominationListLoaded
                   && (Context.NominationListContext.NominationList.HasNominationsForAward(AwardType.StarValues) ||
                       Context.NominationListContext.NominationList.HasNominationsForAward(AwardType.RisingStar));
        }
    }
}