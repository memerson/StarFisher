using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesCertificateSourceExcelFile : CertificateSourceExcelFile
    {
        public StarValuesCertificateSourceExcelFile(AwardWinnerList awardWinnerList)
            : base(awardWinnerList?.Quarter, awardWinnerList?.StarValuesAwardWinners)
        { }
    }
}