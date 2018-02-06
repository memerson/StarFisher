using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Perisistence
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

        public AwardWinnerDto(PerformanceAwardWinnerBase performanceAwardWinner)
            : this((AwardWinnerBase)performanceAwardWinner)
        {
            IsFullTime = performanceAwardWinner.IsFullTime;
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

        public bool IsFullTime { get; set; }

        public List<string> CompanyValues { get; set; }

        public List<string> NominationWriteUps { get; set; }

        internal AwardWinnerBase ToAwardWinner()
        {
            var awardType = ValueObjects.AwardType.Create(AwardType);

            var person = Person.Create(PersonName.Create(Name), ValueObjects.OfficeLocation.Create(OfficeLocation),
                ValueObjects.EmailAddress.Create(EmailAddress));

            if (awardType == ValueObjects.AwardType.RisingPerformance)
                return ToRisingPerformanceAwardWinner(person);
            if (awardType == ValueObjects.AwardType.StarPerformance)
                return ToStarPerformanceAwardWinner(person);
            if (awardType == ValueObjects.AwardType.StarRising)
                return ToStarRisingAwardWinner(person);
            if (awardType == ValueObjects.AwardType.StarValues)
                return ToStarValuesAwardWinner(person);

            return null;
        }

        private RisingPerformanceAwardWinner ToRisingPerformanceAwardWinner(Person person)
        {
            return new RisingPerformanceAwardWinner(Id, person, ValueObjects.AwardAmount.Create(AwardAmount), IsFullTime);
        }

        private StarPerformanceAwardWinner ToStarPerformanceAwardWinner(Person person)
        {
            return new StarPerformanceAwardWinner(Id, person, ValueObjects.AwardAmount.Create(AwardAmount), IsFullTime);
        }

        private StarRisingAwardWinner ToStarRisingAwardWinner(Person person)
        {
            return new StarRisingAwardWinner(Id, person);
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
