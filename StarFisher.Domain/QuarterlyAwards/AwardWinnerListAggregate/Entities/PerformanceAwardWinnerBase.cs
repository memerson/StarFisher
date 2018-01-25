using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public abstract class PerformanceAwardWinnerBase : AwardWinnerBase
    {
        protected PerformanceAwardWinnerBase(int id, Person person, AwardType awardType, AwardAmount awardAmount, bool isFullTime)
            : base(id, person, awardType, awardAmount)
        {
            IsFullTime = isFullTime;
        }

        public bool IsFullTime { get; set; }
    }
}
