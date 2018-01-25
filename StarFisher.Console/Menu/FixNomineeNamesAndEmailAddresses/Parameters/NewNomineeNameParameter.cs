using System;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public class NewNomineeNameParameter : ParameterBase<PersonName>
    {
        private readonly PersonName _oldNomineeName;

        public NewNomineeNameParameter(PersonName oldNomineeName)
        {
            _oldNomineeName = oldNomineeName ?? throw new ArgumentNullException(nameof(oldNomineeName));
        }

        public override Argument<PersonName> GetArgument()
        {
            WriteLine();
            WriteLine($@"Enter the new name for the nominee currently named {_oldNomineeName.FullName}.");
            Write(@"> ");

            var input = ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                return Argument<PersonName>.Invalid;

            if (string.Equals(@"done", input, StringComparison.InvariantCultureIgnoreCase))
                return Argument<PersonName>.Abort;

            return PersonName.IsValid(input)
                ? Argument<PersonName>.Valid(PersonName.Create(input))
                : Argument<PersonName>.Invalid;
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid name. Valid names are like Matthew Joel Emerson or Matthew Emerson.");
        }
    }
}
