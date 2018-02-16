using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Office.Outlook;

namespace StarFisher.Console.Menu.ValidateNomineesWithHr
{
    public class ValidateNomineesWithHrMenuItemCommand : MenuItemCommandBase
    {
        private readonly IEmailFactory _emailFactory;

        private const string CommandTitle = @"Validate nominees with Human Resources";
        private const string SuccessMessage = @"Success! You should now see an email ready to review and send on to Human Resources.";

        public ValidateNomineesWithHrMenuItemCommand(IStarFisherContext context, IEmailFactory emailFactory) 
            : base(context, CommandTitle, SuccessMessage)
        {
            _emailFactory = emailFactory ?? throw new ArgumentNullException(nameof(emailFactory));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;
            using (var email = _emailFactory.GetHumanResourcesNomineeValidationEmail(nominationList))
                email.Display();
            
            return CommandOutput.None.Success;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized && Context.NominationListContext.HasNominationListLoaded;
        }
    }
}
