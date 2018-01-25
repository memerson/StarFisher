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

        public IReadOnlyCollection<Nomination> StarRisingNominees => GetNominationsByAwardType(AwardType.StarRising).ToList();

        public IReadOnlyCollection<Person> AwardsLuncheonInvitees => GetNominationsByAwardType(AwardType.StarValues)
            .Where(n => n.NomineeOfficeLocation == OfficeLocation.NashvilleCorporate ||
                        n.NomineeOfficeLocation == OfficeLocation.HighlandRidge ||
                        n.NomineeOfficeLocation == OfficeLocation.EchoBrentwood)
            .Select(n => n.GetNominee())
            .Distinct()
            .ToList();

        public IReadOnlyCollection<Nomination> StarValuesNominees => GetNominationsByAwardType(AwardType.StarValues).ToList();

        public void UpdateNomineeName(PersonName oldNomineeName, PersonName newNomineeName, bool deriveEmailAddress)
        {
            if(oldNomineeName == null)
                throw new ArgumentNullException(nameof(oldNomineeName));
            if (newNomineeName == null)
                throw new ArgumentNullException(nameof(newNomineeName));

            if (oldNomineeName == newNomineeName)
                return;

            var nominations = Nominations.Where(n => n.NomineeName == oldNomineeName);

            foreach (var nomination in nominations)
            {
                nomination.UpdateNomineeName(newNomineeName, deriveEmailAddress);
                IsDirty = true;
            }
        }

        private IEnumerable<Nomination> GetNominationsByAwardType(AwardType awardType)
        {
            return Nominations.Where(n => n.AwardType == awardType);
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
