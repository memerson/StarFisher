using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class RisingStarNominationNotificationEmailSourceExcelFile : NominationNotificationEmailSourceExcelFileBase
    {
        public RisingStarNominationNotificationEmailSourceExcelFile(NominationList nominationList)
            : base(nominationList?.AwardsPeriod, nominationList?.RisingStarNominations)
        {
        }
    }
}