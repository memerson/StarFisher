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

            RegisterAbortInput(@"done");
        }

        public override Argument<EmailAddress> GetArgument()
        {
            WriteLine();
            WriteLine($@"Enter the email address for the nominee named {_nominee.Name.FullName} from {_nominee.OfficeLocation.ConciseName}, or enter 'done' if you don't want to change it.");
            Write(@"> ");

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid HealthStream email address. Valid email addresses are like matthew.emerson@healthstream.com.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out EmailAddress argumentValue)
        {
            if (EmailAddress.GetIsValid(input))
            {
                argumentValue = EmailAddress.Create(input);
                return true;
            }

            argumentValue = null;
            return false;
        }
    }
}
