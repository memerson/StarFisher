using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook;

namespace StarFisher.Console.Menu.CreateHumanResourceNomineeValidationEmail
{
    public class CreateHumanResourceNomineeValidationEmailMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Create Human Resources nominee validation email";

        private const string SuccessMessage =
            @"Success! You should now see an email ready to review and send on to Human Resources.";

        private readonly IEmailFactory _emailFactory;

        public CreateHumanResourceNomineeValidationEmailMenuItemCommand(IStarFisherContext context,
            IEmailFactory emailFactory)
            : base(context, CommandTitle, SuccessMessage)
        {
            _emailFactory = emailFactory ?? throw new ArgumentNullException(nameof(emailFactory));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;
            using (var email = _emailFactory.GetHumanResourcesNomineeValidationEmail(nominationList))
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