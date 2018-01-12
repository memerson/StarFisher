using System;
using StarFisher.Domain.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public abstract class AwardWinner : Entity
    {
        protected AwardWinner(int id, AwardType awardType, PersonName name, OfficeLocation officeLocation, AwardAmount awardAmount)
            : base(id)
        {
            AwardType = awardType ?? throw new ArgumentNullException(nameof(awardType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            OfficeLocation = officeLocation ?? throw new ArgumentNullException(nameof(officeLocation));
            AwardAmount = awardAmount ?? throw new ArgumentNullException(nameof(awardAmount));
        }

        public AwardType AwardType { get; }

        public PersonName Name { get; }

        public OfficeLocation OfficeLocation { get; }

        public AwardAmount AwardAmount { get; }
    }
}
