using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Persistence
{
    internal class AwardWinnerDto
    {
        internal AwardWinnerDto() { }

        internal AwardWinnerDto(AwardWinnerBase awardWinner)
        {
            if (awardWinner == null)
                throw new ArgumentNullException(nameof(awardWinner));

            Id = awardWinner.Id;
            Name = awardWinner.Name.ToString();
            OfficeLocation = awardWinner.OfficeLocation.ToString();
            EmailAddress = awardWinner.EmailAddress.ToString();
            AwardType = awardWinner.AwardType.ToString();
            AwardAmount = awardWinner.AwardAmount.ValueInDollars;
        }

        public AwardWinnerDto(StarValuesAwardWinner starValuesAwardWinner)
            : this((AwardWinnerBase)starValuesAwardWinner)
        {
            CompanyValues = starValuesAwardWinner.CompanyValues.Select(cv => cv.ToString()).ToList();
            NominationWriteUps = starValuesAwardWinner.NominationWriteUps.Select(wu => wu.ToString()).ToList();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string OfficeLocation { get; set; }

        public string EmailAddress { get; set; }

        public string AwardType { get; set; }

        public decimal AwardAmount { get; set; }

        public List<string> CompanyValues { get; set; }

        public List<string> NominationWriteUps { get; set; }

        internal AwardWinnerBase ToAwardWinner()
        {
            var awardType = ValueObjects.AwardType.Create(AwardType);

            var person = Person.Create(PersonName.Create(Name), ValueObjects.OfficeLocation.Create(OfficeLocation),
                ValueObjects.EmailAddress.Create(EmailAddress));

            if (awardType == ValueObjects.AwardType.RisingStar)
                return ToRisingStarAwardWinner(person);
            if (awardType == ValueObjects.AwardType.StarValues)
                return ToStarValuesAwardWinner(person);

            return null;
        }

        private RisingStarAwardWinner ToRisingStarAwardWinner(Person person)
        {
            return new RisingStarAwardWinner(Id, person);
        }

        private StarValuesAwardWinner ToStarValuesAwardWinner(Person person)
        {
            var companyValues = (CompanyValues ?? Enumerable.Empty<string>())
                .Select(CompanyValue.Create)
                .ToList();

            var writeUps = (NominationWriteUps ?? Enumerable.Empty<string>())
                .Select(wu => NominationWriteUp.Create(person.Name, wu))
                .ToList();

            return new StarValuesAwardWinner(Id, person, companyValues, writeUps);
        }
    }
}
