using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Excel
{
    internal class StarValuesNominationNotificationEmailSourceExcelFile : NominationNotificationEmailSourceExcelFileBase
    {
        public StarValuesNominationNotificationEmailSourceExcelFile(NominationList nominationList) 
            : base(nominationList?.Quarter, nominationList?.StarValuesNominees)
        { }
    }
}
