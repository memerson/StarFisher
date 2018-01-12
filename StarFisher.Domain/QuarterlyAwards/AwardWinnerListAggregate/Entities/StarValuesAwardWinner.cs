using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public class StarValuesAwardWinner : AwardWinner
    {
        private static readonly AwardAmount StarValuesAwardAmount = AwardAmount.Create(350);

        internal StarValuesAwardWinner(int id, PersonName name, OfficeLocation officeLocation, IEnumerable<CompanyValue> companyValues, IEnumerable<NominationWriteUp> nominationWriteUps, EmailAddress emailAddress)
            : base(id, AwardType.StarValues, name, officeLocation, StarValuesAwardAmount)
        {
            CompanyValues = companyValues?.ToList() ?? throw new ArgumentNullException(nameof(companyValues));
            NominationWriteUps = nominationWriteUps?.ToList() ??
                                 throw new ArgumentNullException(nameof(nominationWriteUps));
            EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        }

        public IReadOnlyCollection<CompanyValue> CompanyValues { get; }

        public IReadOnlyCollection<NominationWriteUp> NominationWriteUps { get; }

        public EmailAddress EmailAddress { get; }
    }
}
