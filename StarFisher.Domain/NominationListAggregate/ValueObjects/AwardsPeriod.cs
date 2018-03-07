using System;
using System.IO;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class AwardsPeriod : ValueObject<AwardsPeriod>
    {
        private AwardsPeriod(AwardCategory awardCategory, Year year, Quarter quarter)
        {
            AwardCategory = awardCategory;
            Year = year;
            Quarter = quarter;
        }

        public AwardCategory AwardCategory { get; }

        public Year Year { get; }

        public Quarter Quarter { get; }

        public int Value => Year.Value * 10 + (Quarter?.NumericValue ?? 5);

        public string FilePathFragment
        {
            get
            {
                if (AwardCategory == AwardCategory.QuarterlyAwards)
                    return Path.Combine(Year.ToString(), Quarter.Abbreviation);
                if (AwardCategory == AwardCategory.SuperStarAwards)
                    return Path.Combine(Year.ToString(), @"SuperStar");

                throw GetAwardCategoryNotSupportedException();
            }
        }

        public string FileNamePrefix
        {
            get
            {
                if (AwardCategory == AwardCategory.QuarterlyAwards)
                    return $@"{Year}{Quarter.Abbreviation}";
                if (AwardCategory == AwardCategory.SuperStarAwards)
                    return $@"{Year}";

                throw GetAwardCategoryNotSupportedException();
            }
        }

        public string AwardsName
        {
            get
            {
                if (AwardCategory == AwardCategory.QuarterlyAwards)
                    return $@"{Year} {Quarter.Abbreviation} Star Awards";
                if (AwardCategory == AwardCategory.SuperStarAwards)
                    return $@"{Year} Super Star Awards";

                throw GetAwardCategoryNotSupportedException();
            }
        }

        public static AwardsPeriod CreateForQuarterlyAwards(Year year, Quarter quarter)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));

            return new AwardsPeriod(AwardCategory.QuarterlyAwards, year, quarter);
        }

        public static AwardsPeriod CreateForSuperStarAwards(Year year)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));

            return new AwardsPeriod(AwardCategory.SuperStarAwards, year, Quarter.None);
        }

        public static AwardsPeriod CreateFromValue(int value)
        {
            var year = Year.Create(value / 10);
            var quarterValue = value % 10;

            if (quarterValue < 1 || quarterValue > 4)
                return CreateForSuperStarAwards(year);

            var quarter = Quarter.FindByNumericValue(quarterValue);

            return CreateForQuarterlyAwards(year, quarter);
        }

        protected override bool EqualsCore(AwardsPeriod other)
        {
            return other.Value == Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }

        private NotSupportedException GetAwardCategoryNotSupportedException()
        {
            return new NotSupportedException($@"Unsupported award category: {AwardCategory.Value}");
        }
    }
}