using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class RisingStarCertificatesSourceExcelFile : CertificatesSourceExcelFileBase
    {
        public RisingStarCertificatesSourceExcelFile(NominationList nominationList)
            : base(nominationList?.AwardsPeriod, nominationList?.RisingStarAwardWinners)
        {
        }
    }
}