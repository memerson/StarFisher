using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters
{
    public abstract class GetNomineeToChangeParameterBase : ParameterBase<Person>
    {
        protected GetNomineeToChangeParameterBase(IReadOnlyCollection<Person> allNominees)
        {
            AllNominees = allNominees?.OrderBy(n => n.Name.FullName).ToList() ??
                           throw new ArgumentNullException(nameof(allNominees));

            RegisterAbortInput(@"done");
        }

        public override Argument<Person> GetArgument()
        {
            if (AllNominees.Count == 0)
            {
                WriteLine("There are no nominees.");
                return Argument<Person>.NoValue;
            }

            SolicitInput();

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid selection. I'm looking for one of the numbers next to one of the nominee names.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out Person argumentValue)
        {
            if (int.TryParse(input, out int nameId))
            {
                var index = nameId - 1;
                if (index >= 0 && index < AllNominees.Count)
                {
                    argumentValue = AllNominees[index];
                    return true;
                }
            }

            argumentValue = null;
            return false;
        }

        protected IReadOnlyList<Person> AllNominees { get; }

        protected abstract void SolicitInput();
    }
}