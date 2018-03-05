using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class SuperStarCertificatesSourceExcelFile : CertificatesSourceExcelFileBase
    {
        public SuperStarCertificatesSourceExcelFile(NominationList nominationList)
            : base(nominationList?.AwardsPeriod, nominationList?.SuperStarAwardWinners)
        {
        }
    }
}