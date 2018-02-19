using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesCertificatesSourceExcelFile : CertificatesSourceExcelFileBase
    {
        public StarValuesCertificatesSourceExcelFile(NominationList nominationList)
            : base(nominationList?.Year, nominationList?.Quarter, nominationList?.StarValuesAwardWinners)
        {
        }
    }
}