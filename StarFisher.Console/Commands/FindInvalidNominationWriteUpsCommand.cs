using System.Linq;
using System.Text.RegularExpressions;
using StarFisher.Console.Commands.Common;

namespace StarFisher.Console.Commands
{
    public class FindInvalidNominationWriteUpsCommand : BaseCommand
    {
        private static readonly Regex CommandExpression = new Regex(
            @"^\s*find\sinvalid\snomination\swrite-ups\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public FindInvalidNominationWriteUpsCommand()
            : base(CommandExpression, true) { }

        protected override CommandResult TryExecute(Match commandRegexMatch)
        {
            var nominationList = StarFisherContext.Current.NominationList;
            var nominationsWithInvalidWritUps = nominationList.Nominations.Where(n => !n.WriteUp.IsValid);

            var foundInvalidWriteUp = false;

            foreach (var nomination in nominationsWithInvalidWritUps)
            {
                foundInvalidWriteUp = true;
                System.Console.WriteLine();
                System.Console.WriteLine($@"Nomination #{nomination.Id}:");
                System.Console.WriteLine(nomination.WriteUp);
                System.Console.WriteLine();
                System.Console.Write("Press any key to continue.");
                System.Console.ReadKey();
                System.Console.WriteLine();
            }

            if (!foundInvalidWriteUp)
            {
                System.Console.WriteLine();
                System.Console.WriteLine("All nominations have valid write-ups.");
            }

            System.Console.WriteLine();

            return CommandResult.Success;
        }
    }
}
