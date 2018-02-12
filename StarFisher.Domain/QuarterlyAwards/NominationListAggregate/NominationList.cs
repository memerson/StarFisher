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
        internal NominationList(Year year, Quarter quarter, List<Nomination> nominations)
            : base(CreateKey(year, quarter))
        {
            Quarter = quarter;
            Year = year;
            Nominations = nominations ?? throw new ArgumentNullException(nameof(nominations));

            SetNomineeIdentifiers(Nominations);
        }

        public Quarter Quarter { get; }

        public Year Year { get; }

        public IReadOnlyCollection<Nomination> Nominations { get; }

        public IReadOnlyCollection<Person> Nominees => Nominations.Select(n => n.Nominee).Distinct().ToList();

        public IReadOnlyCollection<Nomination> StarRisingNominees => GetNominationsByAwardType(AwardType.StarRising).ToList();

        public IReadOnlyCollection<Person> AwardsLuncheonInvitees => GetNominationsByAwardType(AwardType.StarValues)
            .Where(n => n.NomineeOfficeLocation == OfficeLocation.NashvilleCorporate ||
                        n.NomineeOfficeLocation == OfficeLocation.HighlandRidge ||
                        n.NomineeOfficeLocation == OfficeLocation.EchoBrentwood)
            .Select(n => n.Nominee)
            .Distinct()
            .ToList();

        public IReadOnlyCollection<Nomination> StarValuesNominees => GetNominationsByAwardType(AwardType.StarValues).ToList();

        public void UpdateNomineeName(Person nominee, PersonName newNomineeName)
        {
            if (nominee == null)
                throw new ArgumentNullException(nameof(nominee));
            if (newNomineeName == null)
                throw new ArgumentNullException(nameof(newNomineeName));
            if (!newNomineeName.IsValid)
                throw new ArgumentException(nameof(newNomineeName));

            if (nominee.Name == newNomineeName)
                return;

            var nominations = Nominations.Where(n => n.Nominee == nominee);

            foreach (var nomination in nominations)
            {
                nomination.UpdateNomineeName(newNomineeName);
                MarkAsDirty($@"Updated nominee name from {nominee.Name.FullName} to {newNomineeName.FullName}");
            }
        }

        public void UpdateNomineeEmailAddress(Person nominee, EmailAddress newEmailAddress)
        {
            if (nominee == null)
                throw new ArgumentNullException(nameof(nominee));
            if (newEmailAddress == null)
                throw new ArgumentNullException(nameof(newEmailAddress));
            if (!newEmailAddress.IsValid)
                throw new ArgumentException(nameof(newEmailAddress));

            if (nominee.EmailAddress == newEmailAddress)
                return;

            var nominations = Nominations.Where(n => n.Nominee == nominee);

            foreach (var nomination in nominations)
            {
                nomination.UpdateNomineeEmailAddress(newEmailAddress);
                MarkAsDirty($@"Updated nominee email address from {nominee.EmailAddress.Value} to {newEmailAddress.Value}");
            }
        }

        public void UpdateNominationWriteUp(int nominationId, NominationWriteUp newWriteUp)
        {
            if (newWriteUp == null)
                throw new ArgumentNullException(nameof(newWriteUp));
            if (!newWriteUp.IsValid)
                throw new ArgumentException(nameof(newWriteUp));

            var nomination = Nominations.FirstOrDefault(n => n.Id == nominationId);

            if (nomination == null)
                throw new ArgumentException(nameof(nominationId));

            nomination.UpdateWriteUp(newWriteUp);
            MarkAsDirty($@"Updated a nomination write-up for {nomination.NomineeName}");
        }

        private IEnumerable<Nomination> GetNominationsByAwardType(AwardType awardType)
        {
            return Nominations.Where(n => n.AwardType == awardType);
        }

        private void SetNomineeIdentifiers(IEnumerable<Nomination> nominations)
        {
            var nominationsByNomineeName = nominations.GroupBy(n => n.NomineeName);
            var updatedVotingIdentifiers = false;

            foreach (var group in nominationsByNomineeName)
            {
                var nomineeIds = group.Select(n => n.Id).ToList();
                var votingIdentifier = NomineeVotingIdentifier.Create(nomineeIds);

                foreach (var nomination in group)
                {
                    if (nomination.VotingIdentifier == votingIdentifier)
                        continue;

                    nomination.SetVotingIdentifier(votingIdentifier);
                    updatedVotingIdentifiers = true;
                }
            }

            if (updatedVotingIdentifiers)
                MarkAsDirty(@"Set nominee voting identifiers.");
        }

        private static int CreateKey(Year year, Quarter quarter)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));

            return year.Value * 10 + quarter.NumericValue;
        }
    }
}
