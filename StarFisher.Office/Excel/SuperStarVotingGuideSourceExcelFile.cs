using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class SuperStarVotingGuideSourceExcelFile : VotingGuideSourceExcelFileBase
    {
        public SuperStarVotingGuideSourceExcelFile(NominationList nominationList)
            : base(nominationList?.SuperStarNominations)
        {
        }
    }
}