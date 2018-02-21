using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.ValueObjects;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate
{
    public class NominationList : AggregateRoot
    {
        private readonly List<AwardWinner> _awardWinners;
        private readonly List<Nomination> _nominations;

        internal NominationList(Year year, Quarter quarter, IEnumerable<Nomination> nominations,
            IEnumerable<AwardWinner> awardWinners = null)
            : base(CreateKey(year, quarter))
        {
            Quarter = quarter;
            Year = year;
            _nominations = nominations?.ToList() ?? throw new ArgumentNullException(nameof(nominations));
            _awardWinners = awardWinners?.ToList() ?? new List<AwardWinner>();

            SetNomineeIdentifiers();
        }

        public Quarter Quarter { get; }

        public Year Year { get; }

        private static int CreateKey(Year year, Quarter quarter)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));

            return year.Value * 10 + quarter.NumericValue;
        }

        #region Nominations

        public IReadOnlyList<Nomination> Nominations => _nominations;

        public IReadOnlyCollection<Person> Nominees => Nominations.Select(n => n.Nominee).Distinct().ToList();

        public IReadOnlyCollection<Person> AwardsLuncheonInvitees => GetNominationsByAwardType(AwardType.StarValues)
            .Where(n => n.NomineeOfficeLocation == OfficeLocation.NashvilleCorporate ||
                        n.NomineeOfficeLocation == OfficeLocation.HighlandRidge ||
                        n.NomineeOfficeLocation == OfficeLocation.EchoBrentwood)
            .Select(n => n.Nominee)
            .Distinct()
            .ToList();

        public IReadOnlyCollection<Nomination> StarValuesNominations => GetNominationsByAwardType(AwardType.StarValues)
            .ToList();

        public IReadOnlyCollection<Nomination> RisingStarNominations => GetNominationsByAwardType(AwardType.RisingStar)
            .ToList();

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

        public IReadOnlyCollection<Person> GetNomineesForAward(AwardType awardType, bool excludeWinners)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            var winners = excludeWinners
                ? GetWinnersForAwardType(awardType).Select(w => w.Person)
                : Enumerable.Empty<Person>();

            return Nominations
                .Where(n => n.AwardType == awardType)
                .Select(n => n.Nominee)
                .Except(winners)
                .ToList();
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
            var updated = false;

            foreach (var nomination in nominations)
            {
                nomination.UpdateNomineeName(newNomineeName);
                updated = true;
            }

            if (!updated)
                return;

            SetNomineeIdentifiers();

            var awardWinner = AwardWinners.FirstOrDefault(w => w.Person == nominee);

            if (awardWinner != null)
                awardWinner.UpdateAwardWinnerName(newNomineeName);

            MarkAsDirty($@"Updated nominee name from {nominee.Name.FullName} to {newNomineeName.FullName}");
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
            var updated = false;

            foreach (var nomination in nominations)
            {
                nomination.UpdateNomineeEmailAddress(newEmailAddress);
                updated = true;
            }

            if (!updated)
                return;

            var awardWinner = AwardWinners.FirstOrDefault(w => w.Person == nominee);

            if (awardWinner != null)
                awardWinner.UpdateAwardWinnerEmailAddress(newEmailAddress);

            MarkAsDirty($@"Updated nominee email address from {nominee.EmailAddress.Value} to {newEmailAddress.Value}");
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

        public void DisqualifyNominee(AwardType awardType, Person nominee)
        {
            if (nominee == null)
                throw new ArgumentNullException(nameof(nominee));

            var removedCount = _nominations.RemoveAll(n => n.AwardType == awardType && n.Nominee == nominee);

            if (removedCount > 0)
            {
                SetNomineeIdentifiers();
                MarkAsDirty($@"Disqualified nominee {nominee.Name.FullName}");
            }

            UnselectAwardWinner(awardType, nominee);
        }

        public void RemoveNomination(int nominationId)
        {
// TODO: Warn if removing only nomination
            var nomination = Nominations.FirstOrDefault(n => n.Id == nominationId);

            if (nomination == null)
                throw new ArgumentException(nameof(nominationId));

            var removed = _nominations.Remove(nomination);

            if (!removed)
                return;

            SetNomineeIdentifiers();
            MarkAsDirty(
                $@"Removed {nomination.NominatorName.RawNameText}'s nomination for {nomination.NomineeName.FullName}");

            if (!GetNominationsForNominee(nomination.AwardType, nomination.Nominee).Any())
                UnselectAwardWinner(nomination.AwardType, nomination.Nominee);
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

        #endregion Nominations

        #region Award Winners

        public IReadOnlyList<AwardWinner> AwardWinners => _awardWinners;

        public IReadOnlyCollection<AwardWinner> RisingStarAwardWinners => GetWinnersForAwardType(AwardType.RisingStar);

        public IReadOnlyCollection<AwardWinner> StarValuesAwardWinners => GetWinnersForAwardType(AwardType.StarValues);

        public bool HasAwardWinners => AwardWinners.Count > 0;

        public bool GetIsAwardWinner(AwardType awardType, Person person)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));
            if (person == null)
                throw new ArgumentNullException(nameof(person));

            var awardWinner = new AwardWinner(awardType, person);

            return AwardWinners.Contains(awardWinner);
        }

        public void UnselectAwardWinner(AwardWinner awardWinner)
        {
            if (awardWinner == null)
                throw new ArgumentNullException(nameof(awardWinner));

            var removed = _awardWinners.Remove(awardWinner);

            if (removed)
                MarkAsDirty($@"Removed {awardWinner.Name.FullName} from winner list for {awardWinner.AwardType.PrettyName}");
        }

        public void SelectNomineeAsAwardWinner(AwardType awardType, Person nominee)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));
            if (nominee == null)
                throw new ArgumentNullException(nameof(nominee));

            if (GetIsAwardWinner(awardType, nominee))
                return;

            var nominations = GetNominationsForNominee(awardType, nominee);
            if (nominations.Count == 0)
                throw new ArgumentException(nameof(nominee));

            var awardWinner = new AwardWinner(awardType, nominee);

            _awardWinners.Add(awardWinner);
            MarkAsDirty($@"Upserted winner {nominee.Name.FullName}");
        }

        public IReadOnlyCollection<CompanyValue> GetCompanyValuesForAwardWinner(AwardWinner awardWinner)
        {
            var nominations = GetNominationsForNominee(awardWinner.AwardType, awardWinner.Person);
            return nominations.SelectMany(n => n.CompanyValues).Distinct().OrderBy(cv => cv.Value).ToList();
        }

        public IReadOnlyCollection<NominationWriteUp> GetNominationWriteUpsForAwardWinner(AwardWinner awardWinner)
        {
            var nominations = GetNominationsForNominee(awardWinner.AwardType, awardWinner.Person);
            return nominations.Select(n => n.WriteUp).ToList();
        }

        private void UnselectAwardWinner(AwardType awardType, Person person)
        {
            var awardWinner = new AwardWinner(awardType, person);
            UnselectAwardWinner(awardWinner);
        }

        private IReadOnlyCollection<AwardWinner> GetWinnersForAwardType(AwardType awardType)
        {
            return AwardWinners.Where(w => w.AwardType == awardType).ToList();
        }

        #endregion Award Winners
    }
}