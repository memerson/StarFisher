using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class RisingStarVotingGuideSourceExcelFile : VotingGuideSourceExcelFileBase
    {
        public RisingStarVotingGuideSourceExcelFile(NominationList nominationList)
            : base(nominationList?.RisingStarNominations)
        {
        }
    }
}