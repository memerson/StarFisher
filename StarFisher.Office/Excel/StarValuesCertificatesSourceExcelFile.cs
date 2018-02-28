using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesCertificatesSourceExcelFile : CertificatesSourceExcelFileBase
    {
        public StarValuesCertificatesSourceExcelFile(NominationList nominationList)
            : base(nominationList?.AwardsPeriod, nominationList?.StarValuesAwardWinners)
        {
        }
    }
}