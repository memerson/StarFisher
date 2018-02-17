using System.Collections.Generic;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Excel
{
    internal class RisingStarCertificateSourceExcelFile : CertificateSourceExcelFileBase
    {
        public RisingStarCertificateSourceExcelFile(AwardWinnerList awardWinnerList, IEnumerable<OfficeLocation> officeLocations)
            : base(awardWinnerList?.Quarter, awardWinnerList?.RisingStarAwardWinners, officeLocations)
        { }
    }
}
