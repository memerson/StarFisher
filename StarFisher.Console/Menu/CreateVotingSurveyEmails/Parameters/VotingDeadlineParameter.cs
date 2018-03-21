using System;
using StarFisher.Console.Menu.Common;

namespace StarFisher.Console.Menu.CreateVotingSurveyEmails.Parameters
{
    public class VotingDeadlineParameter : ParameterBase<DateTime>
    {
        public VotingDeadlineParameter()
        {
            RegisterAbortInput(@"stop");
        }

        public override Argument<DateTime> GetArgumentCore()
        {
            var exampleDate = DateTime.Now + TimeSpan.FromDays(14);

            WriteLine();
            WriteCallToAction(
                $@"Enter the voting deadline date (e.g. '{
                        exampleDate.ToShortDateString()
                    }', or enter 'stop' to stop creating the voting survey emails.");
            WriteInputPrompt();

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid future date.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out DateTime argumentValue)
        {
            if (!DateTime.TryParse(input, out argumentValue))
                return false;

            return argumentValue > DateTime.Now.Date;
        }
    }
}