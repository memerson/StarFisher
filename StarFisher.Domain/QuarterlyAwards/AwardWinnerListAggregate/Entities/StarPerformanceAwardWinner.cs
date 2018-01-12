using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public class StarPerformanceAwardWinner : PerformanceAwardWinner
    {
        public StarPerformanceAwardWinner(int id, PersonName name, OfficeLocation officeLocation, AwardAmount awardAmount, EmailAddress emailAddress, bool isFullTime)
            : base(id, AwardType.StarPerformance, name, officeLocation, awardAmount, emailAddress, isFullTime)
        { }
    }
}
