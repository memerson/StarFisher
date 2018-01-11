using System.Linq;
using System.Text.RegularExpressions;
using StarFisher.Console.Commands.Common;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.Utilities;

namespace StarFisher.Console.Commands
{
    public class ListNomineeNamesCommand : BaseCommand
    {
        private static readonly Regex CommandExpression = new Regex(
            @"^\s*list\snominee\snames\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ListNomineeNamesCommand()
            : base(CommandExpression, true) { }

        protected override CommandResult TryExecute(Match match)
        {
            var nominationList = StarFisherContext.Current.NominationList;
            var nominations = nominationList.Nominations.OrderBy(n => n.NomineeName, PersonNameComparer.FirstNameFirst);

            System.Console.WriteLine();

            foreach (var nomination in nominations)
                System.Console.WriteLine($"{nomination.Id, -5}{nomination.NomineeName.FullName, -30}");

            System.Console.WriteLine();

            return CommandResult.Success;
        }
    }
}
