using System.Text.RegularExpressions;
using StarFisher.Console.Commands.Common;

namespace StarFisher.Console.Commands
{
    public class ExitCommand : BaseCommand
    {
        private static readonly Regex CommandExpression = new Regex(
            @"^\s*exit\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ExitCommand()
            : base(CommandExpression, false)
        {
        }

        protected override CommandResult TryExecute(Match commandRegexMatch)
        {
            return CommandResult.Success;
        }
    }
}
