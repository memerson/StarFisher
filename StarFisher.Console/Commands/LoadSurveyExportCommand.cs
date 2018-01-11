using System.Text.RegularExpressions;
using StarFisher.Console.Commands.Common;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Commands
{
    public class LoadSurveyExportCommand : BaseCommand
    {
        private readonly INominationListRepository _nominationListRepository;

        private static readonly Regex CommandExpression = new Regex(
            @"^\s*load\ssurvey\sexport\sfor\sQ(?<quarter>[1-4])\s20(?<year>[1-9][0-9])\sfrom\s(?<filePath>.*)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public LoadSurveyExportCommand(INominationListRepository nominationListRepository)
            : base(CommandExpression, false)
        {
            _nominationListRepository = nominationListRepository;
        }

        protected override CommandResult TryExecute(Match commandRegexMatch)
        {
            var quarterNumericValue = int.Parse(commandRegexMatch.Groups["quarter"].Value);

            if(!Quarter.IsValid(quarterNumericValue))
                return CommandResult.Error($@"Invalid quarter: {quarterNumericValue}");

            var yearNumericValue = int.Parse(commandRegexMatch.Groups["year"].Value);

            if (!Year.IsValid(yearNumericValue))
                return CommandResult.Error($@"Invalid year: {yearNumericValue}");

            var filePathText = commandRegexMatch.Groups["filePath"].Value;

            if(!FilePath.IsValid(filePathText, true))
                return CommandResult.Error($@"Invalid file path: {filePathText}");

            var quarter = Quarter.Create(quarterNumericValue);
            var year = Year.Create(yearNumericValue);
            var filePath = FilePath.Create(filePathText, true);

            var nominationList = _nominationListRepository.LoadSurveyExport(filePath, quarter, year);
            StarFisherContext.Current.SetContextNominationList(nominationList);

            return CommandResult.Success;
        }
    }
}
