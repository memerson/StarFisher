using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public class StarPerformanceAwardWinner : PerformanceAwardWinnerBase
    {
        public StarPerformanceAwardWinner(int id, Person person, AwardAmount awardAmount, bool isFullTime)
            : base(id, person, AwardType.StarPerformance, awardAmount, isFullTime)
        { }
    }
}
