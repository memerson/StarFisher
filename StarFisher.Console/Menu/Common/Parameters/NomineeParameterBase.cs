using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.Common.Parameters
{
    public abstract class NomineeParameterBase : ListItemSelectionParameterBase<Person>
    {
        protected NomineeParameterBase(IReadOnlyCollection<Person> nominees)
            : base(nominees?.OrderBy(n => n.Name.FullName).ToList(), @"nominees")
        {
        }
    }
}