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
        private readonly List<Nomination> _nominations;

        internal NominationList(Year year, Quarter quarter, List<Nomination> nominations)
            : base(CreateKey(year, quarter))
        {
            Quarter = quarter;
            Year = year;
            _nominations = nominations ?? throw new ArgumentNullException(nameof(nominations));

            SetNomineeIdentifiers();
        }

        public Quarter Quarter { get; }

        public Year Year { get; }

        public IReadOnlyList<Nomination> Nominations => _nominations;

        public IReadOnlyCollection<Person> Nominees => Nominations.Select(n => n.Nominee).Distinct().ToList();

        public IReadOnlyCollection<Person> AwardsLuncheonInvitees => GetNominationsByAwardType(AwardType.StarValues)
            .Where(n => n.NomineeOfficeLocation == OfficeLocation.NashvilleCorporate ||
                        n.NomineeOfficeLocation == OfficeLocation.HighlandRidge ||
                        n.NomineeOfficeLocation == OfficeLocation.EchoBrentwood)
            .Select(n => n.Nominee)
            .Distinct()
            .ToList();

        public IReadOnlyCollection<Nomination> StarValuesNominations => GetNominationsByAwardType(AwardType.StarValues).ToList();

        public IReadOnlyCollection<Nomination> RisingStarNominations => GetNominationsByAwardType(AwardType.RisingStar).ToList();

        public IReadOnlyCollection<Nomination> GetNominationsForNominee(AwardType awardType, Person nominee)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));
            if (nominee == null)
                throw new ArgumentNullException(nameof(nominee));

            return Nominations.Where(n => n.Nominee == nominee && n.AwardType == awardType).ToList();
        }

        public bool HasNominationsForAward(AwardType awardType)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            return Nominations.Any(n => n.AwardType == awardType);
        }

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
            var updatedNominee = false;

            foreach (var nomination in nominations)
            {
                nomination.UpdateNomineeName(newNomineeName);
                updatedNominee = true;
            }

            if (updatedNominee)
            {
                SetNomineeIdentifiers();
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

        public void DisqualifyNominee(Person nominee)
        {
            if (nominee == null)
                throw new ArgumentNullException(nameof(nominee));

            _nominations.RemoveAll(n => n.Nominee == nominee);
            SetNomineeIdentifiers();
            MarkAsDirty($@"Disqualified nominee {nominee.Name.FullName}");
        }

        public void RemoveNomination(int nominationId)
        {
            var nomination = Nominations.FirstOrDefault(n => n.Id == nominationId);

            if (nomination == null)
                throw new ArgumentException(nameof(nominationId));

            _nominations.Remove(nomination);
            SetNomineeIdentifiers();
            MarkAsDirty($@"Removed {nomination.NominatorName.RawNameText}'s nomination for {nomination.NomineeName.FullName}");
        }

        private IEnumerable<Nomination> GetNominationsByAwardType(AwardType awardType)
        {
            return Nominations.Where(n => n.AwardType == awardType);
        }

        private void SetNomineeIdentifiers()
        {
            var nominationsByNominee = Nominations.GroupBy(n => n.Nominee);
            var updatedVotingIdentifiers = false;

            foreach (var group in nominationsByNominee)
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
