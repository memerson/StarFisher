using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StarFisher.Console.Commands.Common;
using StarFisher.Office.Word;

namespace StarFisher.Console.Commands
{
    class CreateStarValuesVotingGuideCommand : BaseCommand
    {
        private readonly IMailMergeFactory _mailMergeFactory;

        private static readonly Regex CommandExpression = new Regex(
            @"^\s*create\sstar\svalues\svoting\sguide\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public CreateStarValuesVotingGuideCommand(IMailMergeFactory mailMergeFactory)
            : base(CommandExpression, true)
        {
            _mailMergeFactory = mailMergeFactory;
        }

        protected override CommandResult TryExecute(Match commandRegexMatch)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("This might take a minute.");
            System.Console.WriteLine();

            var nominationList = StarFisherContext.Current.NominationList;
            var mailMerge = _mailMergeFactory.GetStarValuesVotingGuideMailMerge(nominationList);
            mailMerge.Execute();
            return CommandResult.Success;
        }
    }
}
