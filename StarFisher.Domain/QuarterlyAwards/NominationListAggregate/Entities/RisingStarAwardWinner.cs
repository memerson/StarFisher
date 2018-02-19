using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities
{
    public class RisingStarAwardWinner : AwardWinnerBase
    {
        internal RisingStarAwardWinner(int id, Person person)
            : base(id, person, AwardType.RisingStar, AwardAmount.RisingStar)
        { }
    }
}
