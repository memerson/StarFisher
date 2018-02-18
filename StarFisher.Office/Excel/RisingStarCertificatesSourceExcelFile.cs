using System.Collections.Generic;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Excel
{
    internal class RisingStarCertificatesSourceExcelFile : CertificatesSourceExcelFileBase
    {
        public RisingStarCertificatesSourceExcelFile(AwardWinnerList awardWinnerList)
            : base(awardWinnerList?.Year, awardWinnerList?.Quarter, awardWinnerList?.RisingStarAwardWinners)
        { }
    }
}
