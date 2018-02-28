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
                @"Enter the path to the .xlsx file for the nomination survey export, or enter 'stop' to stop loading the survey export.");
            WriteInputPrompt();

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid file path to a .xlsx file.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out FilePath argumentValue)
        {
            var isXlsx = string.Equals(Path.GetExtension(input), @".xlsx", StringComparison.InvariantCultureIgnoreCase);

            if (isXlsx && FilePath.IsValid(input, true))
            {
                argumentValue = FilePath.Create(input, true);
                return true;
            }

            argumentValue = null;
            return false;
        }
    }
}