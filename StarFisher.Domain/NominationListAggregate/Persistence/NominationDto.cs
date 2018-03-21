using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.NominationListAggregate.Entities;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Domain.NominationListAggregate.Persistence
{
    internal class NominationDto
    {
        internal NominationDto()
        {
        }

        internal NominationDto(Nomination nomination)
        {
            if (nomination == null)
                throw new ArgumentNullException(nameof(nomination));

            Id = nomination.Id;
            NomineeName = nomination.NomineeName.ToString();
            NomineeOfficeLocation = nomination.NomineeOfficeLocation.ToString();
            NomineeEmailAddress = nomination.NomineeEmailAddress.ToString();
            AwardType = nomination.AwardType.ToString();
            NominatorName = nomination.NominatorName.RawNameText;
            IsNominatorAnonymous = nomination.NominatorName.IsAnonymous;
            CompanyValues = nomination.CompanyValues.Select(cv => cv.ToString()).ToList();
            WriteUp = nomination.WriteUp.ToString();
            WriteUpSummary = nomination.WriteUpSummary.ToString();
        }

        public int Id { get; set; }

        public string NomineeName { get; set; }

        public string NomineeOfficeLocation { get; set; }

        public string NomineeEmailAddress { get; set; }

        public string AwardType { get; set; }

        public string NominatorName { get; set; }

        public bool IsNominatorAnonymous { get; set; }

        public List<string> CompanyValues { get; set; }

        public string WriteUp { get; set; }

        public string WriteUpSummary { get; set; }

        internal Nomination ToNomination()
        {
            var nominee = Person.Create(PersonName.Create(NomineeName),
                OfficeLocation.FindByName(NomineeOfficeLocation),
                EmailAddress.Create(NomineeEmailAddress));

            var companyValues = (CompanyValues ?? Enumerable.Empty<string>())
                .Select(CompanyValue.FindByValue)
                .ToList();

            return new Nomination(Id,
                NomineeVotingIdentifier.Unknown,
                nominee,
                ValueObjects.AwardType.FindByAwardName(AwardType),
                PersonName.CreateForNominator(NominatorName, IsNominatorAnonymous),
                companyValues,
                NominationWriteUp.Create(nominee.Name, WriteUp),
                NominationWriteUpSummary.Create(WriteUpSummary));
        }
    }
}