using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Persistence
{
    internal class NominationListDto
    {
        internal NominationListDto() { }

        internal NominationListDto(NominationList nominationList)
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

        internal NominationList ToNominationList()
        {
            var nominations = (Nominations ?? Enumerable.Empty<NominationDto>())
                .Select(n => n.ToNomination())
                .ToList();

            return new NominationList(Domain.ValueObjects.Year.Create(Year),
                Domain.ValueObjects.Quarter.Create(Quarter), nominations);
        }
    }
}
