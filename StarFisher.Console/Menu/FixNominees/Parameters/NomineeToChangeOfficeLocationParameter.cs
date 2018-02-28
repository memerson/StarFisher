using System.Collections.Generic;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.FixNominees.Parameters
{
    public class NomineeToChangeOfficeLocationParameter : NomineeParameterBase
    {
        public NomineeToChangeOfficeLocationParameter(IReadOnlyCollection<Person> allNominees)
            : base(allNominees)
        {
            RegisterAbortInput(@"done");
        }

        protected override string GetListItemLabel(Person listItem)
        {
            return $@"{listItem.Name.FullName,-25} {listItem.OfficeLocation.ConciseName}";
        }

        protected override void WriteCallToAction()
        {
            WriteCallToAction(
                @"Enter the number of the nominee whose office location you want to change, or enter 'done' if you don't want to change any office locations.");
        }
    }
}