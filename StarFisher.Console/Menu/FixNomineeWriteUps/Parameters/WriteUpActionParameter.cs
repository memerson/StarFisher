using System;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeWriteUps.Parameters
{
    public class WriteUpActionParameter : ParameterBase<WriteUpActionParameter.Action>
    {
        private readonly PersonName _nomineeName;
        private readonly NominationWriteUp _writeUp;

        public WriteUpActionParameter(PersonName nomineeName, NominationWriteUp writeUp)
        {
            _nomineeName = nomineeName ?? throw new ArgumentNullException(nameof(nomineeName));
            _writeUp = writeUp ?? throw new ArgumentNullException(nameof(writeUp));

            RegisterValidInput("stop", Action.Stop);
            RegisterValidInput("edit", Action.Edit);
            RegisterValidInput("c", Action.Continue);
        }

        public override Argument<Action> GetArgument()
        {
            WriteLine();
            WriteLineBlue($@"Nomination Write-Up for {_nomineeName.FullName}:");
            WriteLine();
            WriteLine(_writeUp.Value, _nomineeName.FirstName, _nomineeName.LastName);
            WriteLine();
            WriteLine("Please enter 'edit' to edit the write-up, 'c' to continue to the next write-up, or 'stop' to stop reviewing write-ups.");
            Write(@"> ");

            return GetRegisteredValidInputArgument();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid choice.");
        }

        //private void PrintWriteUp()
        //{
        //    WriteLine();

        //    if (!_writeUp.ContainsNomineeName)
        //    {
        //        WriteLine(_writeUp.Value);
        //        WriteLine();
        //        return;
        //    }

        //    var currentIndex = 0;
        //    var text = _writeUp.Value;
        //    var finalIndex = text.Length - 1;

        //    while (currentIndex < text.Length)
        //    {
        //        var nextFirstNameIndex = IndexOf(text, _nomineeName.FirstName, currentIndex);
        //        var nextLastNameIndex = IndexOf(text, _nomineeName.LastName, currentIndex);
        //        var cleanSegmentEndIndex = Math.Min(nextFirstNameIndex, nextLastNameIndex);
        //        var cleanSegmentLength = cleanSegmentEndIndex - currentIndex;
        //        var cleanSegment = text.Substring(currentIndex, cleanSegmentLength);

        //        Write(cleanSegment);

        //        if (cleanSegmentEndIndex >= finalIndex)
        //            return;

        //        var nameSegment = cleanSegmentEndIndex == nextFirstNameIndex
        //            ? _nomineeName.FirstName
        //            : _nomineeName.LastName;

        //        WriteRed(nameSegment);

        //        currentIndex = cleanSegmentEndIndex + nameSegment.Length;
        //    }

        //    WriteLine();
        //}

        private static int IndexOf(string text, string value, int startIndex)
        {
            var index = text.IndexOf(value, startIndex, StringComparison.InvariantCultureIgnoreCase);
            return index == -1 ? text.Length : index;
        }

        public enum Action
        {
            Stop,
            Continue,
            Edit
        }
    }
}