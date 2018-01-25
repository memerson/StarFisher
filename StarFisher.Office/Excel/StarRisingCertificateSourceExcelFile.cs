using System.Collections.Generic;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Excel
{
    internal class StarRisingCertificateSourceExcelFile : CertificateSourceExcelFileBase
    {
        public StarRisingCertificateSourceExcelFile(AwardWinnerList awardWinnerList, IEnumerable<OfficeLocation> officeLocations)
            : base(awardWinnerList?.Quarter, awardWinnerList?.StarRisingAwardWinners, officeLocations)
        { }
    }
}
