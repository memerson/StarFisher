using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.Utilities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate
{
    public class AwardWinnerList : AggregateRoot
    {
        private readonly List<AwardWinnerBase> _awardWinners;

        public AwardWinnerList(Year year, Quarter quarter)
            : base(CreateKey(year, quarter))
        {
            Quarter = quarter;
            Year = year;
            _awardWinners = new List<AwardWinnerBase>();
            MarkAsDirty(@"Initial creation");
        }

        internal AwardWinnerList(Year year, Quarter quarter, IEnumerable<AwardWinnerBase> awardWinners)
            : this(year, quarter)
        {
            _awardWinners.AddRange(awardWinners ?? throw new ArgumentNullException(nameof(awardWinners)));
        }

        public Quarter Quarter { get; }

        public Year Year { get; }

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

        public void UpsertNomineeToWinners(NominationList nominationList, AwardType awardType, Person nominee)
        {
            if (nominationList == null)
                throw new ArgumentNullException(nameof(nominationList));
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));
            if (nominee == null)
                throw new ArgumentNullException(nameof(nominee));

            var nominations = nominationList.GetNominationsForNominee(awardType, nominee);
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

        public void SyncWithUpdatedNomination(NominationList nominationList, Nomination nomination)
        {
            if (nominationList == null)
                throw new ArgumentNullException(nameof(nominationList));
            if (nomination == null)
                throw new ArgumentNullException(nameof(nomination));

            var awardType = nomination.AwardType;
            var nominee = nomination.Nominee;
            if (GetIsWinner(awardType, nominee))
                UpsertNomineeToWinners(nominationList, awardType, nominee);
        }

        public void RemoveWinner(Person winner)
        {
            if (winner == null)
                throw new ArgumentNullException(nameof(winner));

            var removed = _awardWinners.RemoveAll(w => w.Person == winner);

            if (removed > 0)
                MarkAsDirty($@"Removed {winner.Name.FullName} from winner list");
        }

        public void UpdateWinnerName(Person winner, PersonName newWinnerName)
        {
            if (winner == null)
                throw new ArgumentNullException(nameof(winner));
            if (newWinnerName == null)
                throw new ArgumentNullException(nameof(newWinnerName));
            if (!newWinnerName.IsValid)
                throw new ArgumentException(nameof(newWinnerName));

            if (winner.Name == newWinnerName)
                return;

            var awardWinner = AwardWinners.FirstOrDefault(w => w.Person == winner);
            if (awardWinner == null)
                return;

            awardWinner.UpdateWinnerName(newWinnerName);
            MarkAsDirty($@"Updated winner name from {winner.Name.FullName} to {newWinnerName.FullName}");
        }

        public void UpdateWinnerEmailAddress(Person winner, EmailAddress newEmailAddress)
        {
            if (winner == null)
                throw new ArgumentNullException(nameof(winner));
            if (newEmailAddress == null)
                throw new ArgumentNullException(nameof(newEmailAddress));
            if (!newEmailAddress.IsValid)
                throw new ArgumentException(nameof(newEmailAddress));

            if (winner.EmailAddress == newEmailAddress)
                return;

            var awardWinner = AwardWinners.FirstOrDefault(w => w.Person == winner);
            if (awardWinner == null)
                return;

            awardWinner.UpdateWinnerEmailAddress(newEmailAddress);
            MarkAsDirty($@"Updated winner email address from {winner.EmailAddress.Value} to {newEmailAddress.Value}");
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
