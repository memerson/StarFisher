using System.Collections.Generic;
using StarFisher.Domain.NominationListAggregate.Entities;

namespace StarFisher.Domain.Utilities
{
    internal class NominationComparer : IComparer<Nomination>
    {
        public static readonly NominationComparer ByNomineeName = new NominationComparer();

        private NominationComparer()
        {
        }

        public int Compare(Nomination x, Nomination y)
        {
            return PersonNameComparer.FirstNameFirst.Compare(x?.NomineeName, y?.NomineeName);
        }
    }
}