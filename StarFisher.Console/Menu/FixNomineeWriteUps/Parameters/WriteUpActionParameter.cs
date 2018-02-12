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

            RegisterAbortInput(@"stop");
            RegisterValidInput(@"edit", Action.Edit);
            RegisterValidInput(@"c", Action.Continue);
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
            PrintInvalidArgumentMessage(@"That's not a valid option.");
        }

        public enum Action
        {
            Continue,
            Edit
        }
    }
}