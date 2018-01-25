using System.Collections.Generic;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Excel
{
    internal class StarValuesCertificateSourceExcelFile : CertificateSourceExcelFileBase
    {
        public StarValuesCertificateSourceExcelFile(AwardWinnerList awardWinnerList, IEnumerable<OfficeLocation> officeLocations)
            : base(awardWinnerList?.Quarter, awardWinnerList?.StarValuesAwardWinners, officeLocations)
        { }
    }
}