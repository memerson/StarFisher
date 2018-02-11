using System;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.Common.Parameters
{
    public abstract class PersonNameParameterBase : ParameterBase<PersonName>
    {
        private readonly IGlobalAddressList _globalAddressList;

        protected PersonNameParameterBase(IGlobalAddressList globalAddressList)
        {
            _globalAddressList = globalAddressList ?? throw new ArgumentNullException(nameof(globalAddressList));
        }

        public override Argument<PersonName> GetArgument()
        {
            WriteLine();
            WriteLine(GetInstructionsText());
            Write(@"> ");

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid name, or it's not in the global address list. Valid names are like Matthew Joel Emerson or Matthew Emerson.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out PersonName argumentValue)
        {
            if (PersonName.GetIsValid(input))
            {
                var personName = PersonName.Create(input);

                if (_globalAddressList.GetPersonExists(personName))
                {
                    argumentValue = personName;
                    return true;
                }
            }

            argumentValue = null;
            return false;
        }

        protected abstract string GetInstructionsText();
    }
}
