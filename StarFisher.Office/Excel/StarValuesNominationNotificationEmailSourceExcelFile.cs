using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesNominationNotificationEmailSourceExcelFile : NominationNotificationEmailSourceExcelFileBase
    {
        public StarValuesNominationNotificationEmailSourceExcelFile(NominationList nominationList)
            : base(nominationList?.AwardsPeriod, nominationList?.StarValuesNominations)
        {
        }
    }
}