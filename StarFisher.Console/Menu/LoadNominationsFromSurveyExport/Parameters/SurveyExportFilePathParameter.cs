using System;
using System.IO;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.LoadNominationsFromSurveyExport.Parameters
{
    public class SurveyExportFilePathParameter : ParameterBase<FilePath>
    {
        public SurveyExportFilePathParameter()
        {
            RegisterAbortInput(@"stop");
        }

        public override Argument<FilePath> GetArgumentCore()
        {
            WriteLine();
            WriteCallToAction(
                @"Enter the path to the .xls file for the nomination survey export, or enter 'stop' to stop loading the survey export.");
            WriteInputPrompt();

            var argument = GetArgumentFromInputIfValid();

            if (argument.ArgumentType != ArgumentType.Valid)
                return argument;

            WriteLine();
            WriteLine(@"Loading! This might take a minute.");
            return argument;
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid file path to a .xls file.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out FilePath argumentValue)
        {
            var isXls = string.Equals(Path.GetExtension(input), @".xls", StringComparison.InvariantCultureIgnoreCase);

            if (isXls && FilePath.IsValid(input, true))
            {
                argumentValue = FilePath.Create(input, true);
                return true;
            }

            argumentValue = null;
            return false;
        }
    }
}