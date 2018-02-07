using System;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public class NewNomineeNameParameter : ParameterBase<PersonName>
    {
        private readonly Person _nominee;

        public NewNomineeNameParameter(Person nominee)
        {
            _nominee = nominee ?? throw new ArgumentNullException(nameof(nominee));

            RegisterAbortInput(@"done");
        }

        public override Argument<PersonName> GetArgument()
        {
            WriteLine();
            WriteLine($@"Enter the new name for the nominee currently named {_nominee.Name.FullName} from {_nominee.OfficeLocation.ConciseName}, or enter 'done' if you don't want to change it.");
            Write(@"> ");

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid name. Valid names are like Matthew Joel Emerson or Matthew Emerson.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out PersonName argumentValue)
        {
            if (PersonName.GetIsValid(input))
            {
                argumentValue = PersonName.Create(input);
                return true;
            }

            argumentValue = null;
            return false;
        }
    }
}
