using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarRisingVotingKeyExcelFile : VotingKeyExcelFile
    {
        public StarRisingVotingKeyExcelFile(NominationList nominationList) 
            : base(nominationList?.StarRisingNominees) { }
    }
}
