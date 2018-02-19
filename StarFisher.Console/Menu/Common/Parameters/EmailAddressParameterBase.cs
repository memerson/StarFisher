using System;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.Common.Parameters
{
    public abstract class EmailAddressParameterBase : ParameterBase<EmailAddress>
    {
        private readonly EmailAddress _derivedEmailAddress;
        private readonly IGlobalAddressList _globalAddressList;

        protected EmailAddressParameterBase(IGlobalAddressList globalAddressList, PersonName personName)
        {
            _globalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
            _derivedEmailAddress = personName?.DerivedEmailAddress ??
                                   throw new ArgumentNullException(nameof(personName));

            RegisterValidInput(@"default", _derivedEmailAddress);
        }

        public override Argument<EmailAddress> GetArgumentCore()
        {
            WriteLine();
            WriteCallToAction(GetCallToActionText() +
                              $@" Alternatively you can enter 'default' to use the default email address of {
                                      _derivedEmailAddress
                                  }.");
            WriteInputPrompt();

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(
                @"That's not a valid HealthStream email address, or it's not in the global address list. Valid HealthStream email addresses are like matt.emerson@healthstream.com.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out EmailAddress argumentValue)
        {
            if (EmailAddress.GetIsValid(input))
            {
                var emailAddress = EmailAddress.Create(input);

                if (_globalAddressList.GetPersonExists(emailAddress))
                {
                    argumentValue = emailAddress;
                    return true;
                }
            }

            argumentValue = null;
            return false;
        }

        protected abstract string GetCallToActionText();
    }
}