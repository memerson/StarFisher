using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Office.Excel
{
    internal class StarValuesNomineeListExcelFile : NomineeListExcelFileBase
    {
        public StarValuesNomineeListExcelFile(NominationList nominationList)
            : base(nominationList?.GetNomineesForAward(AwardType.StarValues, true))
        {
        }
    }
}