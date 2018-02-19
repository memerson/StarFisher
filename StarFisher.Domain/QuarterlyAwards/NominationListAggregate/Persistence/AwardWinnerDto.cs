using System;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.ValueObjects;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Persistence
{
    internal class AwardWinnerDto
    {
        internal AwardWinnerDto()
        {
        }

        internal AwardWinnerDto(AwardWinner awardWinner)
        {
            if (awardWinner == null)
                throw new ArgumentNullException(nameof(awardWinner));

            AwardType = awardWinner.AwardType.ToString();
            Name = awardWinner.Name.ToString();
            OfficeLocation = awardWinner.OfficeLocation.ToString();
            EmailAddress = awardWinner.EmailAddress.ToString();
        }

        public string AwardType { get; set; }

        public string Name { get; set; }

        public string OfficeLocation { get; set; }

        public string EmailAddress { get; set; }

        internal AwardWinner ToAwardWinner()
        {
            var awardType = Domain.ValueObjects.AwardType.Create(AwardType);

            var person = Person.Create(PersonName.Create(Name),
                Domain.ValueObjects.OfficeLocation.Create(OfficeLocation),
                Domain.ValueObjects.EmailAddress.Create(EmailAddress));

            return new AwardWinner(awardType, person);
        }
    }
}