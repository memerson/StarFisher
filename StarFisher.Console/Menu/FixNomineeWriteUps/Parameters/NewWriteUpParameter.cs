using System;
using System.Diagnostics;
using System.IO;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeWriteUps.Parameters
{
    public class NewWriteUpParameter : ParameterBase<NominationWriteUp>
    {
        private static readonly string TempFilePath = Path.GetTempFileName();
        private readonly PersonName _nomineeName;
        private readonly NominationWriteUp _writeUp;

        public NewWriteUpParameter(PersonName nomineeName, NominationWriteUp writeUp)
        {
            _nomineeName = nomineeName ?? throw new ArgumentNullException(nameof(nomineeName));
            _writeUp = writeUp ?? throw new ArgumentNullException(nameof(writeUp));

            RegisterAbortInput(@"no");
        }

        public override Argument<NominationWriteUp> GetArgumentCore()
        {
            WriteLine();
            WriteLine(
                @"I'm going to open the write-up in Notepad. Please edit it there, then SAVE and CLOSE Notepad. Come back here when you're finished.");
            WriteLine();
            WriteCallToAction(@"Ready? Enter 'no' to back out or 'yes' to continue.");
            WriteInputPrompt();

            if (GetIsAbortInput(out Argument<NominationWriteUp> abortArgument))
                return abortArgument;

            File.WriteAllText(TempFilePath, _writeUp.Value);

            using (var process = Process.Start("c:\\windows\\notepad.exe", $"\"{TempFilePath}\""))
            {
                if (process == null)
                    return Argument<NominationWriteUp>.Invalid;

                WriteLine("I'm waiting on you to SAVE and CLOSE Notepad.");

                process.WaitForExit(int.MaxValue);
            }

            var writeUpText = File.ReadAllText(TempFilePath);
            var argument = GetArgumentFromInputIfValid(writeUpText);

            if (argument.ArgumentType != ArgumentType.Valid)
                return argument;

            WriteLine();
            WriteLine(@"Got it, thanks!");
            WriteLine();

            return argument;
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(
                @"The nomination write-up text wasn't valid. Either it included some part of the nominee's name or it didn't have any text.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out NominationWriteUp argumentValue)
        {
            if (NominationWriteUp.GetIsValid(_nomineeName, input))
            {
                argumentValue = NominationWriteUp.Create(_nomineeName, input);
                return true;
            }

            argumentValue = null;
            return false;
        }
    }
}