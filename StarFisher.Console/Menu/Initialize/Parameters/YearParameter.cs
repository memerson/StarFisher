using StarFisher.Console.Menu.Common;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.Initialize.Parameters
{
    public class YearParameter : ParameterBase<Year>
    {
        public YearParameter()
        {
            RegisterAbortInput(@"stop");
        }

        public override Argument<Year> GetArgumentCore()
        {
            WriteLine();
            WriteCallToAction(
                @"Please specify the year for the award, or enter 'stop' to stop the initialization workflow.");
            WriteInputPrompt();

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid year.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out Year argumentValue)
        {
            if (int.TryParse(input, out int year) && Year.IsValid(year))
            {
                argumentValue = Year.Create(year);
                return true;
            }

            argumentValue = null;
            return false;
        }
    }
}