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
        }

        public override Argument<Action> GetArgument()
        {
            PrintWriteUp();
            WriteLine("Please enter 'edit' to edit the write-up, 'c' to continue to the next write-up, or 'stop' to stop reviewing write-ups.");
            Write(@"> ");

            var input = ReadInput();

            if (string.IsNullOrWhiteSpace(input))
                return Argument<Action>.Invalid;

            if (string.Equals(@"c", input, StringComparison.InvariantCultureIgnoreCase))
                input = "Continue";

            if(!Enum.TryParse(input, true, out Action action))
                return Argument<Action>.Invalid;

            return Argument<Action>.Valid(action);
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid choice.");
        }

        private void PrintWriteUp()
        {
            WriteLine();

            if (!_writeUp.ContainsNomineeName)
            {
                WriteLine(_writeUp.Value);
                WriteLine();
                return;
            }

            var currentIndex = 0;
            var text = _writeUp.Value;
            var finalIndex = text.Length - 1;

            while (currentIndex < text.Length)
            {
                var nextFirstNameIndex = IndexOf(text, _nomineeName.FirstName, currentIndex);
                var nextLastNameIndex = IndexOf(text, _nomineeName.LastName, currentIndex);
                var cleanSegmentEndIndex = Math.Min(nextFirstNameIndex, nextLastNameIndex);
                var cleanSegmentLength = cleanSegmentEndIndex - currentIndex;
                var cleanSegment = text.Substring(currentIndex, cleanSegmentLength);

                Write(cleanSegment);

                if (cleanSegmentEndIndex >= finalIndex)
                    return;

                var nameSegment = cleanSegmentEndIndex == nextFirstNameIndex
                    ? _nomineeName.FirstName
                    : _nomineeName.LastName;

                WriteRed(nameSegment);

                currentIndex = cleanSegmentEndIndex + nameSegment.Length;
            }

            WriteLine();
        }

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