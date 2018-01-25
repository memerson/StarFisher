using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Persistence
{
    internal class NominationDto
    {
        public NominationDto() { }

        public NominationDto(Nomination nomination)
        {
            if(nomination == null)
                throw new ArgumentNullException(nameof(nomination));

            Id = nomination.Id;
            NominationIds = nomination.VotingIdentifier.NominationIds.ToList();
            NomineeName = nomination.NomineeName.ToString();
            NominatorName = nomination.NominatorName.RawNameText;
            IsNominatorAnonymous = nomination.NominatorName.IsAnonymous;
            AwardType = nomination.AwardType.ToString();
            NomineeOfficeLocation = nomination.NomineeOfficeLocation.ToString();
            CompanyValues = nomination.CompanyValues.Select(cv => cv.ToString()).ToList();
            WriteUp = nomination.WriteUp.ToString();
            WriteUpSummary = nomination.WriteUpSummary.ToString();
            NomineeEmailAddress = nomination.NomineeEmailAddress.ToString();
        }

        public int Id { get; set; }

        public List<int> NominationIds { get; set; }

        public string NomineeName { get; set; }

        public string NominatorName { get; set; }

        public bool IsNominatorAnonymous { get; set; }

        public string AwardType { get; set; }

        public string NomineeOfficeLocation { get; set; }

        public List<string> CompanyValues { get; set; }

        public string WriteUp { get; set; }

        public string WriteUpSummary { get; set; }

        public string NomineeEmailAddress { get; set; }

        internal Nomination ToNomination()
        {
            var nomineeName = PersonName.Create(NomineeName);
            var companyValues = (CompanyValues ?? Enumerable.Empty<string>())
                .Select(CompanyValue.Create)
                .ToList();

            return new Nomination(Id,
                NomineeVotingIdentifier.Create(NominationIds),
                nomineeName,
                PersonName.CreateForNominator(NominatorName, IsNominatorAnonymous),
                Domain.ValueObjects.AwardType.Create(AwardType),
                OfficeLocation.Create(NomineeOfficeLocation),
                companyValues,
                NominationWriteUp.Create(nomineeName, WriteUp),
                NominationWriteUpSummary.Create(WriteUpSummary),
                EmailAddress.Create(NomineeEmailAddress));
        }
    }
}
