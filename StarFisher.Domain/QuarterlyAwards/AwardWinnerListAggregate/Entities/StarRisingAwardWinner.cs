using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public class StarRisingAwardWinner : AwardWinnerBase
    {
        internal StarRisingAwardWinner(int id, Person person)
            : base(id, person, AwardType.StarRising, AwardAmount.StarRising)
        { }
    }
}
