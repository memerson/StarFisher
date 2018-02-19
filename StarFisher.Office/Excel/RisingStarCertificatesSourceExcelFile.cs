using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class RisingStarCertificatesSourceExcelFile : CertificatesSourceExcelFileBase
    {
        public RisingStarCertificatesSourceExcelFile(NominationList nominationList)
            : base(nominationList?.Year, nominationList?.Quarter, nominationList?.RisingStarAwardWinners)
        { }
    }
}
