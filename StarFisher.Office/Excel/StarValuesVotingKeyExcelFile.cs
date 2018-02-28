using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesVotingKeyExcelFile : VotingKeyExcelFileBase
    {
        public StarValuesVotingKeyExcelFile(NominationList nominationList)
            : base(nominationList?.StarValuesNominations)
        {
        }
    }
}