using System;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Office.Outlook.AddressBook;

namespace StarFisher.Console.Menu.Initialize.Parameters
{
    public class PersonNameParameter : PersonNameParameterBase
    {
        private readonly string _personTitle;

        public PersonNameParameter(IGlobalAddressList globalAddressList, string personTitle)
            : base(globalAddressList)
        {
            if (string.IsNullOrWhiteSpace(personTitle))
                throw new ArgumentException(nameof(personTitle));

            _personTitle = personTitle;

            RegisterAbortInput(@"stop");
        }

        protected override string GetCallToActionText()
        {
            return $@"Enter the name of the {_personTitle}, or enter 'stop' to stop the initialization workflow.";
        }
    }
}
