using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public class RisingPerformanceAwardWinner : PerformanceAwardWinnerBase
    {
        public RisingPerformanceAwardWinner(int id, Person person, AwardAmount awardAmount, bool isFullTime)
            : base(id, person, AwardType.RisingPerformance, awardAmount, isFullTime)
        { }
    }
}
