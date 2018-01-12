using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Persistence;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities
{
    public class Nomination : Entity
    {
        internal Nomination(int id, NomineeVotingIdentifier votingIdentifier, PersonName nomineeName, PersonName nominatorName, AwardType nomineeAwardType, OfficeLocation nomineeOfficeLocation, IEnumerable<CompanyValue> companyValues, NominationWriteUp writeUp, NominationWriteUpSummary writeUpSummary, EmailAddress nomineeEmailAddress)
            : base(id)
        {
            VotingIdentifier = votingIdentifier;
            NominatorName = nominatorName ?? throw new ArgumentNullException(nameof(nominatorName));
            NomineeName = nomineeName ?? throw new ArgumentNullException(nameof(nomineeName));
            AwardType = nomineeAwardType ?? throw new ArgumentNullException(nameof(nomineeAwardType));
            NomineeOfficeLocation = nomineeOfficeLocation ??
                                    throw new ArgumentNullException(nameof(nomineeOfficeLocation));
            CompanyValues = companyValues?.ToList() ?? new List<CompanyValue>();
            WriteUp = writeUp;
            WriteUpSummary = writeUpSummary;
            NomineeEmailAddress = nomineeEmailAddress ?? throw new ArgumentNullException(nameof(nomineeEmailAddress));
        }

        public NomineeVotingIdentifier VotingIdentifier { get; private set; }

        public PersonName NominatorName { get; }

        public PersonName NomineeName { get; }

        public AwardType AwardType { get; }

        public OfficeLocation NomineeOfficeLocation { get; }

        public IReadOnlyCollection<CompanyValue> CompanyValues { get; }

        public NominationWriteUp WriteUp { get; }

        public NominationWriteUpSummary WriteUpSummary { get; }

        public EmailAddress NomineeEmailAddress { get; private set; }

        internal void SetVotingIdentifier(NomineeVotingIdentifier votingIdentifier)
        {
            VotingIdentifier = votingIdentifier ?? throw new ArgumentNullException(nameof(votingIdentifier));
        }
    }
}
