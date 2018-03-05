using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class SuperStarVotingKeyExcelFile : VotingKeyExcelFileBase
    {
        public SuperStarVotingKeyExcelFile(NominationList nominationList)
            : base(nominationList?.SuperStarNominations)
        {
        }
    }
}