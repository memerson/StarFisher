using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesVotingKeyExcelFile : VotingKeyExcelFile
    {
        public StarValuesVotingKeyExcelFile(NominationList nominationList)
            : base(nominationList?.StarValuesNominees) { }
    }
}
