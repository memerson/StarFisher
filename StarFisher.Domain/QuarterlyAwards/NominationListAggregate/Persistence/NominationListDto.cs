
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Persistence
{
    internal class NominationListDto
    {
        public NominationListDto() { }

        public NominationListDto(NominationList nominationList)
        {
            if(nominationList == null)
                throw new ArgumentNullException(nameof(nominationList));

            Quarter = nominationList.Quarter.NumericValue;
            Year = nominationList.Year.Value;
            Nominations = nominationList.Nominations.Select(n => new NominationDto(n)).ToList();
        }

        public int Quarter { get; set; }

        public int Year { get; set; }

        public List<NominationDto> Nominations { get; set; }

        public NominationList ToNominationList()
        {
            return new NominationList(Domain.ValueObjects.Quarter.Create(Quarter),
                Domain.ValueObjects.Year.Create(Year), Nominations.Select(n => n.ToNomination()).ToList());
        }
    }
}
