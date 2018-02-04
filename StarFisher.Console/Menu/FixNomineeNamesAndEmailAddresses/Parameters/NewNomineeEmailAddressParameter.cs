using System;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public class NewNomineeEmailAddressParameter : ParameterBase<EmailAddress>
    {
        private readonly Person _nominee;

        public NewNomineeEmailAddressParameter(Person nominee)
        {
            _nominee = nominee ?? throw new ArgumentNullException(nameof(nominee));
        }

        public override Argument<EmailAddress> GetArgument()
        {
            WriteLine();
            WriteLine($@"Enter the email address for the nominee named {_nominee.Name.FullName} from {_nominee.OfficeLocation.ConciseName}, or enter 'done' if you don't want to change it.");
            Write(@"> ");

            var input = ReadInput();

            if (string.IsNullOrWhiteSpace(input))
                return Argument<EmailAddress>.Invalid;

            if (string.Equals(@"done", input, StringComparison.InvariantCultureIgnoreCase))
                return Argument<EmailAddress>.Abort;

            return EmailAddress.GetIsValid(input)
                ? Argument<EmailAddress>.Valid(EmailAddress.Create(input))
                : Argument<EmailAddress>.Invalid;
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid HealthStream email address. Valid email addresses are like matthew.emerson@healthstream.com.");
        }
    }
}
