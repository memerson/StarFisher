using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public class StarValuesAwardWinner : AwardWinnerBase
    {
        internal StarValuesAwardWinner(int id, Person person, IEnumerable<CompanyValue> companyValues, IEnumerable<NominationWriteUp> nominationWriteUps)
            : base(id, person, AwardType.StarValues, AwardAmount.StarValues)
        {
            CompanyValues = companyValues?.ToList() ?? throw new ArgumentNullException(nameof(companyValues));
            NominationWriteUps = nominationWriteUps?.ToList() ??
                                 throw new ArgumentNullException(nameof(nominationWriteUps));
        }

        public IReadOnlyCollection<CompanyValue> CompanyValues { get; }

        public IReadOnlyCollection<NominationWriteUp> NominationWriteUps { get; }
    }
}
