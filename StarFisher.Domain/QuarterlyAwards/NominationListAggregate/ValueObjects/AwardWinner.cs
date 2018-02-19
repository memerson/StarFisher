using System;
using StarFisher.Domain.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate.ValueObjects
{
    public class AwardWinner : ValueObject<AwardWinner>
    {
        internal AwardWinner(AwardType awardType, Person person)
        {
            AwardType = awardType ?? throw new ArgumentNullException(nameof(awardType));
            Person = person ?? throw new ArgumentNullException(nameof(person));
        }

        public AwardType AwardType { get; }

        internal Person Person { get; private set; }

        public PersonName Name => Person.Name;

        public OfficeLocation OfficeLocation => Person.OfficeLocation;

        public EmailAddress EmailAddress => Person.EmailAddress;

        internal void UpdateAwardWinnerName(PersonName newWinnerName)
        {
            Person = Person.UpdateName(newWinnerName);
        }

        internal void UpdateAwardWinnerEmailAddress(EmailAddress newEmailAddress)
        {
            Person = Person.UpdateEmailAddress(newEmailAddress);
        }

        protected override bool EqualsCore(AwardWinner other)
        {
            return AwardType == other.AwardType && Person == other.Person;
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                var hashCode = AwardType.GetHashCode();
                hashCode = (hashCode * 397) ^ Person.GetHashCode();
                return hashCode;
            }
        }
    }
}