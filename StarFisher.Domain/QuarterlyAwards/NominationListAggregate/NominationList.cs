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
        private readonly List<AwardWinnerBase> _awardWinners;

        internal NominationList(Year year, Quarter quarter, IEnumerable<Nomination> nominations, IEnumerable<AwardWinnerBase> awardWinners = null)
            : base(CreateKey(year, quarter))
        {
            Quarter = quarter;
            Year = year;
            _nominations = nominations?.ToList() ?? throw new ArgumentNullException(nameof(nominations));
            _awardWinners = awardWinners?.ToList() ?? new List<AwardWinnerBase>();

            SetNomineeIdentifiers();
        }

        public Quarter Quarter { get; }

        public Year Year { get; }

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
                awardWinner.UpdateWinnerName(newNomineeName);

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
                awardWinner.UpdateWinnerEmailAddress(newEmailAddress);

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

            if(GetIsWinner(nomination.AwardType, nomination.Nominee))
                SyncWinnerWithUpdatedNomination(nomination);
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

            RemoveWinner(awardType, nominee);
        }

        public void RemoveNomination(int nominationId)
        {// TODO: Warn if removing only nomination
            var nomination = Nominations.FirstOrDefault(n => n.Id == nominationId);

            if (nomination == null)
                throw new ArgumentException(nameof(nominationId));

            var removedNomination = _nominations.Remove(nomination);

            if (!removedNomination)
                return;

            SetNomineeIdentifiers();
            MarkAsDirty($@"Removed {nomination.NominatorName.RawNameText}'s nomination for {nomination.NomineeName.FullName}");

            if (GetNominationsForNominee(nomination.AwardType, nomination.Nominee).Any())
                SyncWinnerWithUpdatedNomination(nomination);
            else
                RemoveWinner(nomination.AwardType, nomination.Nominee);
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

        public IReadOnlyCollection<AwardWinnerBase> AwardWinners => _awardWinners;

        public IReadOnlyCollection<RisingStarAwardWinner> RisingStarAwardWinners => GetAwardWinnersOfType<RisingStarAwardWinner>();

        public IReadOnlyCollection<StarValuesAwardWinner> StarValuesAwardWinners => GetAwardWinnersOfType<StarValuesAwardWinner>();

        public bool GetIsWinner(AwardType awardType, Person person)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));
            if (person == null)
                throw new ArgumentNullException(nameof(person));

            return AwardWinners.Any(w => w.AwardType == awardType && w.Person == person);
        }

        public void RemoveWinner(AwardType awardType, Person winner)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));
            if (winner == null)
                throw new ArgumentNullException(nameof(winner));

            var removedCount = _awardWinners.RemoveAll(w => w.AwardType == awardType && w.Person == winner);

            if (removedCount > 0)
                MarkAsDirty($@"Removed {winner.Name.FullName} from winner list");
        }

        public void UpsertNomineeToWinners(AwardType awardType, Person nominee)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));
            if (nominee == null)
                throw new ArgumentNullException(nameof(nominee));

            var nominations = GetNominationsForNominee(awardType, nominee);
            if (nominations.Count == 0)
                throw new ArgumentException(nameof(nominee));

            var companyValues = nominations.SelectMany(n => n.CompanyValues).Distinct().OrderBy(cv => cv.Value).ToList();
            var nominationWriteUps = nominations.Select(n => n.WriteUp).ToList();

            var id = GetNextAwardWinnerId();

            AwardWinnerBase winner;
            if (awardType == AwardType.StarValues)
                winner = new StarValuesAwardWinner(id, nominee, companyValues, nominationWriteUps);
            else if (awardType == AwardType.RisingStar)
                winner = new RisingStarAwardWinner(id, nominee);
            else
                throw new NotSupportedException(awardType.Value);

            _awardWinners.RemoveAll(w => w.Person == nominee);
            _awardWinners.Add(winner);
            MarkAsDirty($@"Upserted winner {nominee.Name.FullName}");
        }

        private void SyncWinnerWithUpdatedNomination(Nomination nomination)
        {
            if (nomination == null)
                throw new ArgumentNullException(nameof(nomination));

            var awardType = nomination.AwardType;
            var nominee = nomination.Nominee;
            if (GetIsWinner(awardType, nominee))
                UpsertNomineeToWinners(awardType, nominee);
        }

        private int GetNextAwardWinnerId()
        {
            return AwardWinners.SafeMax(w => w.Id) + 1;
        }

        private List<T> GetAwardWinnersOfType<T>()
            where T : AwardWinnerBase
        {
            return _awardWinners.Where(w => w is T).Cast<T>().ToList();
        }

        #endregion Award Winners

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
