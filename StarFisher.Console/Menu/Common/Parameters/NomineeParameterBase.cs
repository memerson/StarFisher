using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.Common.Parameters
{
    public abstract class NomineeParameterBase : ListItemSelectionParameterBase<Person>
    {
        protected NomineeParameterBase(IReadOnlyCollection<Person> allNominees)
            : base(allNominees?.OrderBy(n => n.Name.FullName).ToList(), @"nominees")
        { }
    }
}