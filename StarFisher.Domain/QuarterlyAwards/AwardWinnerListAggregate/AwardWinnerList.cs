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
        private readonly List<AwardWinner> _awardWinners;

        public AwardWinnerList(Quarter quarter, Year year)
            : base(CreateKey(quarter, year))
        {
            Quarter = quarter;
            Year = year;
            _awardWinners = new List<AwardWinner>();
            IsDirty = true;
        }

        internal AwardWinnerList(Quarter quarter, Year year, IEnumerable<AwardWinner> awardWinners)
            : this(quarter, year)
        {
            _awardWinners.AddRange(awardWinners ?? throw new ArgumentNullException(nameof(awardWinners)));
            IsDirty = false;
        }

        internal bool IsDirty { get; private set; }

        public Quarter Quarter { get; }

        public Year Year { get; }

        public IReadOnlyCollection<AwardWinner> AwardWinners => _awardWinners;

        public IReadOnlyCollection<StarRisingAwardWinner> StarRisingAwardWinners => GetAwardWinnersOfType<StarRisingAwardWinner>();

        public IReadOnlyCollection<StarValuesAwardWinner> StarValuesAwardWinners => GetAwardWinnersOfType<StarValuesAwardWinner>();

        public IReadOnlyCollection<PerformanceAwardWinner> PerformanceAwardWinners => GetAwardWinnersOfType<PerformanceAwardWinner>();

        //public void AddWinner(AwardType awardType, PersonName name, OfficeLocation officeLocation, EmailAddress emailAddress)
        //{
        //    var id = AwardWinners.SafeMax(w => w.Id) + 1;
        //    var winner = new AwardWinner(id, awardType, name, officeLocation, emailAddress);
        //    _awardWinners.Add(winner);
        //    IsDirty = true;
        //}

        public void AddStarValuesWinner(PersonName name, OfficeLocation officeLocation, ICollection<CompanyValue> companyValues, ICollection<NominationWriteUp> nominationWriteUps, EmailAddress emailAddress)
        {
            if (companyValues?.Count == 0)
                throw new ArgumentException("A winner must exhibit at least one company value.");
            if (nominationWriteUps?.Count == 0)
                throw new ArgumentException("A winner must have at least one nomination write-up.");

            var id = AwardWinners.SafeMax(w => w.Id) + 1;
            var winner = new StarValuesAwardWinner(id,
                name, officeLocation, companyValues, nominationWriteUps, emailAddress);

            _awardWinners.Add(winner);
            IsDirty = true;
        }

        private List<T> GetAwardWinnersOfType<T>()
            where T : AwardWinner
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
