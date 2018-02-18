using System.Collections.Generic;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Excel
{
    internal class StarValuesCertificatesSourceExcelFile : CertificatesSourceExcelFileBase
    {
        public StarValuesCertificatesSourceExcelFile(AwardWinnerList awardWinnerList)
            : base(awardWinnerList?.Year, awardWinnerList?.Quarter, awardWinnerList?.StarValuesAwardWinners)
        { }
    }
}