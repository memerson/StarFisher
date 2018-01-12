using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate
{
    public class NominationList : AggregateRoot
    {
        internal NominationList(Quarter quarter, Year year, List<Nomination> nominations)
            : base(CreateKey(quarter, year))
        {
            Quarter = quarter;
            Year = year;
            Nominations = nominations ?? throw new ArgumentNullException(nameof(nominations));

            SetNomineeIdentifiers(Nominations);
            IsDirty = true; // From setting the nominee identifiers
        }

        internal bool IsDirty { get; private set; }

        public Quarter Quarter { get; }

        public Year Year { get; }

        public IReadOnlyCollection<Nomination> Nominations { get; }

        public IReadOnlyCollection<Nomination> StarRisingNominees => GetNominationsByAwardType(AwardType.StarRising);

        public IReadOnlyCollection<Nomination> AwardsLuncheonInvitees => GetNominationsByAwardType(AwardType.StarValues);

        public IReadOnlyCollection<Nomination> StarValuesNominees => GetNominationsByAwardType(AwardType.StarValues);

        private List<Nomination> GetNominationsByAwardType(AwardType awardType)
        {
            return Nominations.Where(n => n.AwardType == awardType).ToList();
        }

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

        private static int CreateKey(Quarter quarter, Year year)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));

            return year.Value * 10 + quarter.NumericValue;
        }
    }
}
