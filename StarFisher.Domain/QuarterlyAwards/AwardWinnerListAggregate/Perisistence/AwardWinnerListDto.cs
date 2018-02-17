using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Perisistence
{
    internal class AwardWinnerListDto
    {
        internal AwardWinnerListDto() { }

        internal AwardWinnerListDto(AwardWinnerList awardWinnerList)
        {
            if (awardWinnerList == null)
                throw new ArgumentNullException(nameof(awardWinnerList));

            LastChangeSummary = awardWinnerList.LastChangeSummary;
            Quarter = awardWinnerList.Quarter.NumericValue;
            Year = awardWinnerList.Year.Value;
            AwardWinners = awardWinnerList.AwardWinners.Select(GetAwardWinnerDto).ToList();
        }

        public string LastChangeSummary { get; set; }

        public int Quarter { get; set; }

        public int Year { get; set; }

        public List<AwardWinnerDto> AwardWinners { get; set; }

        internal AwardWinnerList ToAwardWinnerList()
        {
            var awardWinners = (AwardWinners ?? Enumerable.Empty<AwardWinnerDto>())
                .Select(aw => aw.ToAwardWinner())
                .ToList();

            return new AwardWinnerList(ValueObjects.Year.Create(Year), ValueObjects.Quarter.Create(Quarter), awardWinners);
        }

        private AwardWinnerDto GetAwardWinnerDto(AwardWinnerBase awardWinner)
        {
            if(awardWinner is StarValuesAwardWinner)
                return new AwardWinnerDto((StarValuesAwardWinner)awardWinner);
            
            return new AwardWinnerDto(awardWinner);
        }
    }
}
