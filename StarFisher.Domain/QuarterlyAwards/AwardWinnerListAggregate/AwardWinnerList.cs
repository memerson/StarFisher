using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities;
using StarFisher.Domain.Utilities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate
{
    public class AwardWinnerList : AggregateRoot
    {
        private readonly List<AwardWinnerBase> _awardWinners;

        public AwardWinnerList(Quarter quarter, Year year)
            : base(CreateKey(quarter, year))
        {
            Quarter = quarter;
            Year = year;
            _awardWinners = new List<AwardWinnerBase>();
            IsDirty = true;
        }

        internal AwardWinnerList(Quarter quarter, Year year, IEnumerable<AwardWinnerBase> awardWinners)
            : this(quarter, year)
        {
            _awardWinners.AddRange(awardWinners ?? throw new ArgumentNullException(nameof(awardWinners)));
            IsDirty = false;
        }

        internal bool IsDirty { get; private set; }

        public Quarter Quarter { get; }

        public Year Year { get; }

        public IReadOnlyCollection<AwardWinnerBase> AwardWinners => _awardWinners;

        public IReadOnlyCollection<StarRisingAwardWinner> StarRisingAwardWinners => GetAwardWinnersOfType<StarRisingAwardWinner>();

        public IReadOnlyCollection<StarValuesAwardWinner> StarValuesAwardWinners => GetAwardWinnersOfType<StarValuesAwardWinner>();

        public IReadOnlyCollection<PerformanceAwardWinnerBase> PerformanceAwardWinners => GetAwardWinnersOfType<PerformanceAwardWinnerBase>();

        public void AddStarValuesWinner(Person person, ICollection<CompanyValue> companyValues, ICollection<NominationWriteUp> nominationWriteUps)
        {
            if (companyValues?.Count == 0)
                throw new ArgumentException("A winner must exhibit at least one company value.");
            if (nominationWriteUps?.Count == 0)
                throw new ArgumentException("A winner must have at least one nomination write-up.");

            var id = GetNextAwardWinnerId();
            var winner = new StarValuesAwardWinner(id, person, companyValues, nominationWriteUps);
            _awardWinners.Add(winner);
            IsDirty = true;
        }

        public void AddStarPerformanceAwardWinner(Person person, AwardAmount awardAmount, bool isFullTime)
        {
            // TODO: Internalize business logic of award amounts
            var id = GetNextAwardWinnerId();
            var winner = new StarPerformanceAwardWinner(id, person, awardAmount, isFullTime);
            _awardWinners.Add(winner);
            IsDirty = true;
        }

        public void AddRisingPerformanceAwardWinner(Person person, AwardAmount awardAmount, bool isFullTime)
        {
            // TODO: Internalize business logic of award amounts
            var id = GetNextAwardWinnerId();
            var winner = new RisingPerformanceAwardWinner(id, person, awardAmount, isFullTime);
            _awardWinners.Add(winner);
            IsDirty = true;
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
