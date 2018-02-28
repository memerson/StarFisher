using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Domain.NominationListAggregate.Entities
{
    public class Nomination : Entity
    {
        internal Nomination(int id, NomineeVotingIdentifier votingIdentifier, Person nominee,
            AwardType nomineeAwardType, PersonName nominatorName, IEnumerable<CompanyValue> companyValues,
            NominationWriteUp writeUp, NominationWriteUpSummary writeUpSummary)
            : base(id)
        {
            VotingIdentifier = votingIdentifier;
            Nominee = nominee ?? throw new ArgumentNullException(nameof(nominee));
            AwardType = nomineeAwardType ?? throw new ArgumentNullException(nameof(nomineeAwardType));
            NominatorName = nominatorName ?? throw new ArgumentNullException(nameof(nominatorName));
            CompanyValues = companyValues?.ToList() ?? new List<CompanyValue>();
            WriteUp = writeUp;
            WriteUpSummary = writeUpSummary;
        }

        public NomineeVotingIdentifier VotingIdentifier { get; private set; }

        internal Person Nominee { get; private set; }

        public PersonName NomineeName => Nominee.Name;

        public OfficeLocation NomineeOfficeLocation => Nominee.OfficeLocation;

        public EmailAddress NomineeEmailAddress => Nominee.EmailAddress;

        public AwardType AwardType { get; }

        public PersonName NominatorName { get; }

        public IReadOnlyCollection<CompanyValue> CompanyValues { get; }

        public NominationWriteUp WriteUp { get; private set; }

        public NominationWriteUpSummary WriteUpSummary { get; }

        internal void UpdateNomineeName(PersonName newNomineeName)
        {
            Nominee = Nominee.UpdateName(newNomineeName);
        }

        internal void UpdateNomineeOfficeLocation(OfficeLocation newOfficeLocation)
        {
            Nominee = Nominee.UpdateOfficeLocation(newOfficeLocation);
        }

        internal void UpdateNomineeEmailAddress(EmailAddress newEmailAddress)
        {
            Nominee = Nominee.UpdateEmailAddress(newEmailAddress);
        }

        internal void UpdateWriteUp(NominationWriteUp newWriteUp)
        {
            WriteUp = newWriteUp;
        }

        internal void SetVotingIdentifier(NomineeVotingIdentifier votingIdentifier)
        {
            VotingIdentifier = votingIdentifier ?? throw new ArgumentNullException(nameof(votingIdentifier));
        }
    }
}