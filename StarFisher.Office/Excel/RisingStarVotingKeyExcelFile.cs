using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class RisingStarVotingKeyExcelFile : VotingKeyExcelFileBase
    {
        public RisingStarVotingKeyExcelFile(NominationList nominationList)
            : base(nominationList?.RisingStarNominations)
        {
        }
    }
}