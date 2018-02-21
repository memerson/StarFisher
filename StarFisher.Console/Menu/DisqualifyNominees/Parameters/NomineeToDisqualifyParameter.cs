using System.Collections.Generic;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.DisqualifyNominees.Parameters
{
    public class NomineeToDisqualifyParameter : NomineeParameterBase
    {
        public NomineeToDisqualifyParameter(IReadOnlyCollection<Person> nominees) : base(nominees)
        {
            RegisterAbortInput(@"stop");
        }

        protected override string GetListItemLabel(Person listItem)
        {
            return $@"{listItem.Name.FullName} ({listItem.OfficeLocation.ConciseName})";
        }

        protected override void WriteCallToAction()
        {
            WriteCallToAction(
                @"Enter the number of the nominee you want to disqualify, or enter 'stop' to stop disqualifying nominees.");
        }
    }
}