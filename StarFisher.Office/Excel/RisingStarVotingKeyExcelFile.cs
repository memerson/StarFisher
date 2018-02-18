using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class RisingStarVotingKeyExcelFile : VotingKeyExcelFileBase
    {
        public RisingStarVotingKeyExcelFile(NominationList nominationList) 
            : base(nominationList?.RisingStarNominations) { }
    }
}
