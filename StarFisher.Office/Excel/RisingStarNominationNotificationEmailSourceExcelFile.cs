using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Excel
{
    internal class RisingStarNominationNotificationEmailSourceExcelFile : NominationNotificationEmailSourceExcelFileBase
    {
        public RisingStarNominationNotificationEmailSourceExcelFile(NominationList nominationList)
            : base(nominationList?.Quarter, nominationList?.RisingStar)
        { }
    }
}
