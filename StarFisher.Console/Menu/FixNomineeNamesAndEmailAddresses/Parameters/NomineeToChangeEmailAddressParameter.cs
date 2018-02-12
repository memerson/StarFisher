using System;
using System.Collections.Generic;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public class NomineeToChangeEmailAddressParameter : NomineeToChangeParameterBase
    {
        private readonly HashSet<EmailAddress> _unrecognizedEmailAddresses;

        public NomineeToChangeEmailAddressParameter(IReadOnlyCollection<Person> allNominees, IReadOnlyCollection<EmailAddress> unrecognizedEmailAddresses)
            : base(allNominees)
        {
            _unrecognizedEmailAddresses =
                new HashSet<EmailAddress>(unrecognizedEmailAddresses ??
                                        throw new ArgumentNullException(nameof(unrecognizedEmailAddresses)));
        }

        protected override void SolicitInput()
        {
            WriteLine();
            WriteLineBlue(
                @"Here are the nominee email addresses. Email addresses in red aren't in the global address list and so are probably wrong.",
                @"red");
            WriteLine();
            WriteLine();

            for (var i = 0; i < AllNominees.Count; ++i)
            {
                if (i != 0 && i % 20 == 0)
                {
                    Write(@"Press any key to continue.");
                    WaitForKeyPress();
                    ClearLastLine();
                }

                var nominee = AllNominees[i];
                var text = $@"{i + 1, 5}: {nominee.EmailAddress} ({nominee.Name.FullName} from {nominee.OfficeLocation.ConciseName})";

                if (_unrecognizedEmailAddresses.Contains(nominee.EmailAddress))
                    WriteLineRed(text);
                else
                    WriteLine(text);
            }

            WriteLine();
            WriteLine(@"Enter the number of the email address you want to change, or enter 'done' if you don't want to modify any email addresses.");
            Write(@"> ");
        }
    }
}
