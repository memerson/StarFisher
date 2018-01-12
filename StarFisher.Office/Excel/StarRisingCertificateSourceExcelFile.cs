using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarRisingCertificateSourceExcelFile : CertificateSourceExcelFile
    {
        public StarRisingCertificateSourceExcelFile(AwardWinnerList awardWinnerList)
            : base(awardWinnerList?.Quarter, awardWinnerList?.StarRisingAwardWinners)
        { }
    }
}
