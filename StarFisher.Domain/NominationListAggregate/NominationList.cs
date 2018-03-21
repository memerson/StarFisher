using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;
using StarFisher.Domain.NominationListAggregate.Entities;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Domain.Utilities;

namespace StarFisher.Domain.NominationListAggregate
{
    public class NominationList : AggregateRoot
    {
        private readonly List<AwardWinner> _awardWinners;
        private readonly List<Nomination> _nominations;

        internal NominationList(AwardsPeriod awardsPeriod, IEnumerable<Nomination> nominations,
            IEnumerable<AwardWinner> awardWinners = null)
            : base(awardsPeriod?.Value ?? -1)
        {
            AwardsPeriod = awardsPeriod ?? throw new ArgumentNullException(nameof(awardsPeriod));
            _nominations = nominations?.ToList() ?? throw new ArgumentNullException(nameof(nominations));
            _awardWinners = awardWinners?.ToList() ?? new List<AwardWinner>();

            SetNomineeIdentifiers();
        }

        public AwardsPeriod AwardsPeriod { get; }

        public AwardCategory AwardCategory => AwardsPeriod.AwardCategory;

        #region Nominations

        public IReadOnlyList<Nomination> Nominations => _nominations;

        public IReadOnlyList<Person> Nominees => Nominations.Select(n => n.Nominee).Distinct().ToList();

        public IReadOnlyList<Person> AwardsLuncheonInvitees
        {
            get
            {
                AwardType awardType;

                if (AwardCategory == AwardCategory.QuarterlyAwards)
                    awardType = AwardType.StarValues;
                else if (AwardCategory == AwardCategory.SuperStarAwards)
                    awardType = AwardType.SuperStar;
                else
                    return new List<Person>();

                return GetNominationsByAwardType(awardType)
                    .Where(n => n.NomineeOfficeLocation == OfficeLocation.NashvilleCorporate ||
                                n.NomineeOfficeLocation == OfficeLocation.HighlandRidge ||
                                n.NomineeOfficeLocation == OfficeLocation.EchoBrentwood)
                    .Select(n => n.Nominee)
                    .Distinct()
                    .ToList();
            }
        }

        public IReadOnlyList<Nomination> StarValuesNominations => GetNominationsByAwardType(AwardType.StarValues)
            .ToList();

        public IReadOnlyList<Nomination> RisingStarNominations => GetNominationsByAwardType(AwardType.RisingStar)
            .ToList();

        public IReadOnlyList<Nomination> SuperStarNominations => GetNominationsByAwardType(AwardType.SuperStar)
            .ToList();

        public bool HasNominations => _nominations.Count > 0;

        public IReadOnlyList<Nomination> GetNominationsForNominee(AwardType awardType, Person nominee)
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

        public IReadOnlyList<Person> GetNomineesForAward(AwardType awardType, bool excludeWinners)
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

        public bool GetIsOnlyNominationForAwardAndNominee(Nomination nomination)
        {
            if (nomination == null)
                throw new ArgumentNullException(nameof(nomination));
            if (!_nominations.Contains(nomination))
                throw new ArgumentException(nameof(nomination));

            return _nominations.Count(n => n.AwardType == nomination.AwardType && n.Nominee == nomination.Nominee) < 2;
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

        public void UpdateNomineeOfficeLocation(Person nominee, OfficeLocation newOfficeLocation)
        {
            if (nominee == null)
                throw new ArgumentNullException(nameof(nominee));
            if (newOfficeLocation == null)
                throw new ArgumentNullException(nameof(newOfficeLocation));
            if (!OfficeLocation.ValidEmployeeOfficeLocations.Contains(newOfficeLocation))
                throw new ArgumentException(nameof(newOfficeLocation));

            var nominations = Nominations.Where(n => n.Nominee == nominee);
            var updated = false;

            foreach (var nomination in nominations)
            {
                nomination.UpdateNomineeOfficeLocation(newOfficeLocation);
                updated = true;
            }

            if (!updated)
                return;

            SetNomineeIdentifiers();

            var awardWinner = AwardWinners.FirstOrDefault(w => w.Person == nominee);

            if (awardWinner != null)
                awardWinner.UpdateAwardWinnerOfficeLocation(newOfficeLocation);

            MarkAsDirty(
                $@"Updated nominee {nominee.Name.FullName}'s office location from {nominee.OfficeLocation.Name} to {
                        newOfficeLocation.Name
                    }");
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

            SetNomineeIdentifiers();

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
            SortNominations();

            var awardTypes = AwardType.ValidAwardTypes.Where(at => at.AwardCategory == AwardCategory);

            foreach (var awardType in awardTypes)
                SetNomineeIdentifiers(awardType);
        }

        private void SetNomineeIdentifiers(AwardType awardType)
        {
            var nominationsByNominee = Nominations
                .Where(n => n.AwardType == awardType)
                .Select((n, i) => new {Index = i, Nomination = n})
                .GroupBy(x => x.Nomination.Nominee);

            foreach (var group in nominationsByNominee)
            {
                var nomineeIndexes = group.Select(x => x.Index).ToList();
                var votingIdentifier = NomineeVotingIdentifier.Create(nomineeIndexes);

                foreach (var item in group)
                {
                    var nomination = item.Nomination;

                    if (nomination.VotingIdentifier == votingIdentifier)
                        continue;

                    nomination.SetVotingIdentifier(votingIdentifier);
                }
            }
        }

        private void SortNominations()
        {
            _nominations.Sort(NominationComparer.ByNomineeName);
        }

        #endregion Nominations

        #region Award Winners

        public IReadOnlyList<AwardWinner> AwardWinners => _awardWinners;

        public IReadOnlyList<AwardWinner> RisingStarAwardWinners => GetWinnersForAwardType(AwardType.RisingStar);

        public IReadOnlyList<AwardWinner> StarValuesAwardWinners => GetWinnersForAwardType(AwardType.StarValues);

        public IReadOnlyList<AwardWinner> SuperStarAwardWinners => GetWinnersForAwardType(AwardType.SuperStar);

        public bool HasAwardWinners => AwardWinners.Count > 0;

        public bool HasRisingStarAwardWinners => RisingStarAwardWinners.Count > 0;

        public bool HasStarValuesAwardWinners => StarValuesAwardWinners.Count > 0;

        public bool HasSuperStarAwardWinners => SuperStarAwardWinners.Count > 0;

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
                MarkAsDirty(
                    $@"Removed {awardWinner.Name.FullName} from winner list for {awardWinner.AwardType.PrettyName}");
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
                throw new ArgumentException(@"There is no such nominee for that award type.", nameof(awardType));

            var awardWinner = new AwardWinner(awardType, nominee);

            _awardWinners.Add(awardWinner);
            MarkAsDirty($@"Upserted winner {nominee.Name.FullName}");
        }

        public IReadOnlyList<CompanyValue> GetCompanyValuesForAwardWinner(AwardWinner awardWinner)
        {
            var nominations = GetNominationsForNominee(awardWinner.AwardType, awardWinner.Person);
            return nominations.SelectMany(n => n.CompanyValues).Distinct().OrderBy(cv => cv.Value).ToList();
        }

        public IReadOnlyList<NominationWriteUp> GetNominationWriteUpsForAwardWinner(AwardWinner awardWinner)
        {
            var nominations = GetNominationsForNominee(awardWinner.AwardType, awardWinner.Person);
            return nominations.Select(n => n.WriteUp).ToList();
        }

        private void UnselectAwardWinner(AwardType awardType, Person person)
        {
            var awardWinner = new AwardWinner(awardType, person);
            UnselectAwardWinner(awardWinner);
        }

        private IReadOnlyList<AwardWinner> GetWinnersForAwardType(AwardType awardType)
        {
            return AwardWinners.Where(w => w.AwardType == awardType).ToList();
        }

        #endregion Award Winners
    }
}