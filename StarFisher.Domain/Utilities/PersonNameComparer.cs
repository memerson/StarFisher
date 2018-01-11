using System;
using System.Collections.Generic;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.Utilities
{
    public static class PersonNameComparer
    {
        public static readonly IComparer<PersonName> FirstNameFirst = new ByFirstName();
        public static readonly IComparer<PersonName> LastNameFirst = new ByLastName();

        internal class ByFirstName : IComparer<PersonName>
        {
            internal ByFirstName() { }

            public int Compare(PersonName x, PersonName y)
            {
                return string.Compare(x?.FullName, y?.FullName, StringComparison.InvariantCulture);
            }
        }

        internal class ByLastName : IComparer<PersonName>
        {
            internal ByLastName() { }

            public int Compare(PersonName x, PersonName y)
            {
                return string.Compare(x?.FullNameLastNameFirst, y?.FullNameLastNameFirst, StringComparison.InvariantCulture);
            }
        }
    }
}
