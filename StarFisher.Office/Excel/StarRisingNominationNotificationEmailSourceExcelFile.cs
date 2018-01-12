using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Excel
{
    internal class StarRisingNominationNotificationEmailSourceExcelFile : NominationNotificationEmailSourceExcelFile
    {
        public StarRisingNominationNotificationEmailSourceExcelFile(NominationList nominationList)
            : base(nominationList?.Quarter, nominationList?.StarRisingNominees)
        { }
    }
}
