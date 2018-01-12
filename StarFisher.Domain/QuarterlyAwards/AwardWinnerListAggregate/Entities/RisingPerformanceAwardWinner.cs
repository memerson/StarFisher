using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public class RisingPerformanceAwardWinner : PerformanceAwardWinner
    {
        public RisingPerformanceAwardWinner(int id, PersonName name, OfficeLocation officeLocation, AwardAmount awardAmount, EmailAddress emailAddress, bool isFullTime)
            : base(id, AwardType.RisingPerformance, name, officeLocation, awardAmount, emailAddress, isFullTime)
        { }
    }
}
