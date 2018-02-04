using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public class NomineeToChangeEmailAddressParameter : ParameterBase<Person>
    {
        private readonly List<Person> _allNominees;
        private readonly HashSet<EmailAddress> _unrecognizedEmailAddresses;

        public NomineeToChangeEmailAddressParameter(IReadOnlyCollection<Person> allNominees, IReadOnlyCollection<EmailAddress> unrecognizedEmailAddresses)
        {
            _allNominees = allNominees?.OrderBy(n => n.Name.FullName).ToList() ??
                           throw new ArgumentNullException(nameof(allNominees));

            _unrecognizedEmailAddresses =
                new HashSet<EmailAddress>(unrecognizedEmailAddresses ??
                                        throw new ArgumentNullException(nameof(unrecognizedEmailAddresses)));
        }

        public override Argument<Person> GetArgument()
        {
            if (_allNominees.Count == 0)
            {
                WriteLine("There are no nominees.");
                return Argument<Person>.NoValue;
            }

            SolicitInput();

            var input = ReadInput();

            if (string.IsNullOrWhiteSpace(input))
                return Argument<Person>.Invalid;

            if (string.Equals(@"done", input, StringComparison.InvariantCultureIgnoreCase))
                return Argument<Person>.Abort;

            var isInt = int.TryParse(input, out int nameId);

            if (!isInt || nameId < 1 || nameId > _allNominees.Count)
                return Argument<Person>.Invalid;

            var nominee = _allNominees[nameId - 1];

            return Argument<Person>.Valid(nominee);
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid selection. I'm looking for one of the numbers next to one of the nominee names.");
        }

        private void SolicitInput()
        {
            WriteLine();
            Write(@"Here are the nominee email addresses. Email addresses in ");
            WriteRed(@"red");
            Write(@" aren't in the global address list and so are probably wrong.");
            WriteLine();
            WriteLine();

            for (var i = 0; i < _allNominees.Count; ++i)
            {
                if (i != 0 && i % 20 == 0)
                {
                    Write(@"Press any key to continue.");
                    WaitForKeyPress();
                    ClearLastLine();
                }

                var nominee = _allNominees[i];
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
