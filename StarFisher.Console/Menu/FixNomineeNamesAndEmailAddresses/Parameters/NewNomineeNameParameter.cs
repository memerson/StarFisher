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
        }

        public override Argument<PersonName> GetArgument()
        {
            WriteLine();
            WriteLine($@"Enter the new name for the nominee currently named {_nominee.Name.FullName} from {_nominee.OfficeLocation.ConciseName}, or enter 'done' if you don't want to change it.");
            Write(@"> ");

            var input = ReadInput();

            if (string.IsNullOrWhiteSpace(input))
                return Argument<PersonName>.Invalid;

            if (string.Equals(@"done", input, StringComparison.InvariantCultureIgnoreCase))
                return Argument<PersonName>.Abort;

            return PersonName.GetIsValid(input)
                ? Argument<PersonName>.Valid(PersonName.Create(input))
                : Argument<PersonName>.Invalid;
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid name. Valid names are like Matthew Joel Emerson or Matthew Emerson.");
        }
    }
}
