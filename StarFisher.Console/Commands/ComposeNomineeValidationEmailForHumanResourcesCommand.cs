using System.Text.RegularExpressions;
using StarFisher.Console.Commands.Common;
using StarFisher.Office.Outlook;

namespace StarFisher.Console.Commands
{
    public class ComposeNomineeValidationEmailForHumanResourcesCommand : BaseCommand
    {
        private readonly IEmailFactory _emailFactory;

        private static readonly Regex CommandExpression = new Regex(
            @"^\s*compose\snominee\svalidation\semail\sfor\sHR\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ComposeNomineeValidationEmailForHumanResourcesCommand(IEmailFactory emailFactory)
            : base(CommandExpression, true)
        {
            _emailFactory = emailFactory;
        }

        protected override CommandResult TryExecute(Match match)
        {
            var email = _emailFactory.GetHumanResourcesNomineeValidationEmail(StarFisherContext.Current.NominationList);
            email.Display();

            return CommandResult.Success;
        }
    }
}
