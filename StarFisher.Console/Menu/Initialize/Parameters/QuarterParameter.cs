using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.Initialize.Parameters
{
    public class QuarterParameter : ParameterBase<Quarter>
    {
        public QuarterParameter()
        {
            RegisterAbortInput(@"stop");
        }

        public override Argument<Quarter> GetArgument()
        {
            WriteLine();
            WriteLine(@"Please specify the quarter for the award ('1', '2', '3', or '4'), or enter 'stop' to stop the initialization workflow.");
            Write(@"> ");

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid quarter.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out Quarter argumentValue)
        {
            if (int.TryParse(input, out int year) && Quarter.IsValid(year))
            {
                argumentValue = Quarter.Create(year);
                return true;
            }

            argumentValue = null;
            return false;
        }
    }
}
