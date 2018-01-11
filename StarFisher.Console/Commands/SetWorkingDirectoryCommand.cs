using System.Text.RegularExpressions;
using StarFisher.Console.Commands.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Commands
{
    public class SetWorkingDirectoryCommand : BaseCommand
    {
        private static readonly Regex CommandExpression = new Regex(
            @"^\s*set\sworking\sdirectory\sto\s(?<directoryPath>.+)\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public SetWorkingDirectoryCommand()
            : base(CommandExpression, false) { }

        protected override CommandResult TryExecute(Match commandRegexMatch)
        {
            var directoryPath = commandRegexMatch.Groups["directoryPath"].Value;

            if(!DirectoryPath.IsValid(directoryPath))
                return CommandResult.Error("Invalid directory path.");

            StarFisherContext.Current.SetWorkingDirectoryPath(DirectoryPath.Create(directoryPath));
            return CommandResult.Success;
        }
    }
}
