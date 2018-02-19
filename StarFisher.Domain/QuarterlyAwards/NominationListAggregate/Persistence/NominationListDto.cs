using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Persistence
{
    internal class NominationListDto
    {
        internal NominationListDto() { }

        internal NominationListDto(NominationList nominationList)
        {
            if(nominationList == null)
                throw new ArgumentNullException(nameof(nominationList));

            LastChangeSummary = nominationList.LastChangeSummary;
            Quarter = nominationList.Quarter.NumericValue;
            Year = nominationList.Year.Value;
            Nominations = nominationList.Nominations.Select(n => new NominationDto(n)).ToList();
            AwardWinners = nominationList.AwardWinners.Select(GetAwardWinnerDto).ToList();
        }

        public string LastChangeSummary { get; set; }

        public int Quarter { get; set; }

        public int Year { get; set; }

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

            return new NominationList(Domain.ValueObjects.Year.Create(Year),
                Domain.ValueObjects.Quarter.Create(Quarter), nominations, awardWinners);
        }

        private AwardWinnerDto GetAwardWinnerDto(AwardWinnerBase awardWinner)
        {
            if (awardWinner is StarValuesAwardWinner)
                return new AwardWinnerDto((StarValuesAwardWinner)awardWinner);

            return new AwardWinnerDto(awardWinner);
        }
    }
}
