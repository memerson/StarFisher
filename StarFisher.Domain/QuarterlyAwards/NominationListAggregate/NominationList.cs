using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.Utilities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate
{
    public class NominationList : AggregateRoot
    {
        private readonly List<Nomination> _nominations;

        internal NominationList(Quarter quarter, Year year, List<Nomination> nominations)
            : base(year.Value * 10 + quarter.NumericValue)
        {
            Quarter = quarter;
            Year = year;
            _nominations = nominations ?? throw new ArgumentNullException(nameof(nominations));

            SetNomineeIdentifiers(_nominations);
            IsDirty = true; // From setting the nominee identifiers
        }

        internal bool IsDirty { get; private set; }

        public Quarter Quarter { get; }

        public Year Year { get; }

        public IReadOnlyCollection<Nomination> Nominations => _nominations;

        private static void SetNomineeIdentifiers(IEnumerable<Nomination> nominations)
        {
            var nominationsByNomineeName = nominations.GroupBy(n => n.NomineeName);

            foreach (var group in nominationsByNomineeName)
            {
                var nomineeIds = group.Select(n => n.Id).ToList();
                var votingIdentifier = NomineeVotingIdentifier.Create(nomineeIds);

                foreach(var nominee in group)
                        nominee.SetVotingIdentifier(votingIdentifier);
            }
        }
    }
}
