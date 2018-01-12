using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public class StarRisingAwardWinner : AwardWinner
    {
        private static readonly AwardAmount StarRisingAwardAmount = AwardAmount.Create(100);

        internal StarRisingAwardWinner(int id, PersonName name, OfficeLocation officeLocation)
            : base(id, AwardType.StarRising, name, officeLocation, StarRisingAwardAmount)
        { }
    }
}
