using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class RisingStarNominationNotificationEmailSourceExcelFile : NominationNotificationEmailSourceExcelFileBase
    {
        public RisingStarNominationNotificationEmailSourceExcelFile(NominationList nominationList)
            : base(nominationList?.Year, nominationList?.Quarter, nominationList?.RisingStarNominations)
        {
        }
    }
}