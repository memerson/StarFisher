using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public class NomineeNameToChangeParameter : ParameterBase<PersonName>
    {
        private readonly List<PersonName> _allNomineeNames;
        private readonly HashSet<PersonName> _unrecognizedNomineeNames;

        public NomineeNameToChangeParameter(IReadOnlyCollection<PersonName> allNomineeNames, IReadOnlyCollection<PersonName> unrecognizedNomineeNames)
        {
            _allNomineeNames = allNomineeNames?.OrderBy(n => n.FullName).ToList() ??
                           throw new ArgumentNullException(nameof(allNomineeNames));

            _unrecognizedNomineeNames =
                new HashSet<PersonName>(unrecognizedNomineeNames ??
                                        throw new ArgumentNullException(nameof(unrecognizedNomineeNames)));

            if (_allNomineeNames.Count == 0)
                throw new ArgumentException(nameof(allNomineeNames));
        }

        public override Argument<PersonName> GetArgument()
        {
            SolicitInput();

            var input = ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                return Argument<PersonName>.Invalid;

            if (string.Equals(@"done", input, StringComparison.InvariantCultureIgnoreCase))
                return Argument<PersonName>.Abort;

            var isInt = int.TryParse(input, out int nameId);

            if (!isInt || nameId < 1 || nameId > _allNomineeNames.Count)
                return Argument<PersonName>.Invalid;

            var personName = _allNomineeNames[nameId - 1];

            return Argument<PersonName>.Valid(personName);
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid selection. I'm looking for one of the numbers next to one of the nominee names.");
        }

        private void SolicitInput()
        {
            WriteLine();
            WriteLine(@"Here are the nominee names:");
            WriteLine();

            for (var i = 0; i < _allNomineeNames.Count; ++i)
            {
                if (i != 0 && i % 20 == 0)
                {
                    Write(@"Press any key to continue.");
                    WaitForKeyPress();
                    ClearLastLine();
                }

                var nomineeName = _allNomineeNames[i];
                var text = $@"{i + 1}: {nomineeName.FullName,-5}";

                if (_unrecognizedNomineeNames.Contains(nomineeName))
                    WriteLineRed(text);
                else
                    WriteLine(text);
            }

            WriteLine();
            WriteLine(@"Enter the number of the name you want to change, or type 'done' if you don't want to modify any names.");
            Write(@"> ");
        }
    }
}
