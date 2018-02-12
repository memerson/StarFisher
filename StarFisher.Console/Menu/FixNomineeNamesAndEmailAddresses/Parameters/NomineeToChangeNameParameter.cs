using System;
using System.Collections.Generic;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public class NomineeToChangeNameParameter : NomineeToChangeParameterBase
    {
        private readonly HashSet<PersonName> _unrecognizedNomineeNames;

        public NomineeToChangeNameParameter(IReadOnlyCollection<Person> allNominees, IReadOnlyCollection<PersonName> unrecognizedNomineeNames)
            : base(allNominees)
        {
            _unrecognizedNomineeNames =
                new HashSet<PersonName>(unrecognizedNomineeNames ??
                                        throw new ArgumentNullException(nameof(unrecognizedNomineeNames)));
        }

        protected override void SolicitInput()
        {
            WriteLine();
            WriteLineBlue(
                @"Here are the nominee names. Names in red aren't in the global address list and so are probably wrong.",
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
                var text = $@"{i + 1, 5}: {nominee.Name.FullName} ({nominee.OfficeLocation.ConciseName})";

                if (_unrecognizedNomineeNames.Contains(nominee.Name))
                    WriteLineRed(text);
                else
                    WriteLine(text);
            }

            WriteLine();
            WriteLine(@"Enter the number of the name you want to change, or enter 'done' if you don't want to modify any names.");
            Write(@"> ");
        }
    }
}
