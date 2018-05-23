using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Office.Excel
{
    internal class RisingStarNomineeListExcelFile : NomineeListExcelFileBase
    {
        public RisingStarNomineeListExcelFile(NominationList nominationList)
            : base(nominationList?.GetNomineesForAward(AwardType.RisingStar, true))
        {
        }
    }
}