using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class SuperStarNominationNotificationEmailSourceExcelFile : NominationNotificationEmailSourceExcelFileBase
    {
        public SuperStarNominationNotificationEmailSourceExcelFile(NominationList nominationList)
            : base(nominationList?.AwardsPeriod, nominationList?.SuperStarNominations)
        {
        }
    }
}