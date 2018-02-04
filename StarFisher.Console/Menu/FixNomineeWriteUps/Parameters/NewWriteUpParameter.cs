using System;
using System.Diagnostics;
using System.IO;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeWriteUps.Parameters
{
    public class NewWriteUpParameter : ParameterBase<NominationWriteUp>
    {
        private readonly PersonName _nomineeName;
        private readonly NominationWriteUp _writeUp;

        public NewWriteUpParameter(PersonName nomineeName, NominationWriteUp writeUp)
        {
            _nomineeName = nomineeName ?? throw new ArgumentNullException(nameof(nomineeName));
            _writeUp = writeUp ?? throw new ArgumentNullException(nameof(writeUp));
        }

        public override Argument<NominationWriteUp> GetArgument()
        {
            WriteLine();
            WriteLine(@"I'm going to open the write-up in Notepad. Please edit it there, then SAVE and CLOSE Notepad. Come back here when you're finished.");
            Write(@"Ready? (enter 'no' to back out or 'yes' to continue) > ");

            var input = ReadInput();

            if (string.Equals(@"no", input, StringComparison.InvariantCultureIgnoreCase))
                return Argument<NominationWriteUp>.Abort;

            var filePath = Path.GetTempFileName();
            File.WriteAllText(filePath, _writeUp.Value);

            using (var process = Process.Start("c:\\windows\\notepad.exe", $"\"{filePath}\""))
            {
                if(process == null)
                    return Argument<NominationWriteUp>.Invalid;

                WriteLine("I'm waiting on you to SAVE and CLOSE Notepad.");

                process.WaitForExit(int.MaxValue);
            }

            var newNominationWriteUpText = File.ReadAllText(filePath);
            var newWriteUp = NominationWriteUp.Create(_nomineeName, newNominationWriteUpText);

            if (!NominationWriteUp.GetIsValid(_nomineeName, newNominationWriteUpText))
                return Argument<NominationWriteUp>.Invalid;

            WriteLine();
            WriteLine(@"Got it, thanks!");
            WriteLine();

            return Argument<NominationWriteUp>.Valid(newWriteUp);
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"The nomination write-up text wasn't valid. Either it included some part of the nominee's name or it didn't have any text.");
        }
    }
}
