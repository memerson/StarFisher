using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFisher.Domain.NominationListAggregate.Persistence
{
    internal class NominationListDto
    {
        internal NominationListDto()
        {
        }

        internal NominationListDto(NominationList nominationList)
        {
            if (nominationList == null)
                throw new ArgumentNullException(nameof(nominationList));

            LastChangeSummary = nominationList.LastChangeSummary;
            AwardsPeriod = nominationList.AwardsPeriod.Value;
            Nominations = nominationList.Nominations.Select(n => new NominationDto(n)).ToList();
            AwardWinners = nominationList.AwardWinners.Select(w => new AwardWinnerDto(w)).ToList();
        }

        public string LastChangeSummary { get; set; }

        public int AwardsPeriod { get; set; }

        public List<NominationDto> Nominations { get; set; }

        public List<AwardWinnerDto> AwardWinners { get; set; }

        internal NominationList ToNominationList()
        {
            var nominations = (Nominations ?? Enumerable.Empty<NominationDto>())
                .Select(n => n.ToNomination())
                .ToList();

            var awardWinners = (AwardWinners ?? Enumerable.Empty<AwardWinnerDto>())
                .Select(aw => aw.ToAwardWinner())
                .ToList();

            return new NominationList(ValueObjects.AwardsPeriod.CreateFromValue(AwardsPeriod), nominations,
                awardWinners);
        }
    }
}