using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesVotingKeyExcelFile : VotingKeyExcelFileBase
    {
        public StarValuesVotingKeyExcelFile(NominationList nominationList)
            : base(nominationList?.StarValuesNominations) { }
    }
}
