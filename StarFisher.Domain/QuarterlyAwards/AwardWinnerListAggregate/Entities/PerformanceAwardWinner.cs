using System;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public abstract class PerformanceAwardWinner : AwardWinner
    {
        protected PerformanceAwardWinner(int id, AwardType awardType, PersonName name, OfficeLocation officeLocation,
            AwardAmount awardAmount, EmailAddress emailAddress, bool isFullTime)
            : base(id, awardType, name, officeLocation, awardAmount)
        {
            EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
            IsFullTime = isFullTime;
        }

        public EmailAddress EmailAddress { get; }

        public bool IsFullTime { get; set; }
    }
}
