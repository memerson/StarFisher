using System;
using StarFisher.Console.Menu.Common;

namespace StarFisher.Console.Menu.CreateVotingSurveyReviewEmail.Parameters
{
    public class VotingSurveyWebLinkParameter : ParameterBase<string>
    {
        public VotingSurveyWebLinkParameter()
        {
            RegisterAbortInput(@"stop");
        }

        public override Argument<string> GetArgumentCore()
        {
            WriteLine();
            WriteIntroduction(
                @"Please enter the web link for the Survey Monkey voting survey, or enter 'stop' to stop creating the voting survey review email.");
            WriteInputPrompt();

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid Survey Monkey URL.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out string argumentValue)
        {
            if (input.StartsWith(@"https://www.surveymonkey.com/r/", StringComparison.InvariantCultureIgnoreCase))
            {
                argumentValue = input;
                return true;
            }

            argumentValue = null;
            return false;
        }
    }
}