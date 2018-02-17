using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class RisingStarVotingGuideSourceExcelFile : VotingGuideSourceExcelFileBase
    {
        public RisingStarVotingGuideSourceExcelFile(NominationList nominationList)
            : base(nominationList?.RisingStar) { }
    }
}
