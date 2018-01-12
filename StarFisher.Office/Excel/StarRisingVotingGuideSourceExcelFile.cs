using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarRisingVotingGuideSourceExcelFile : VotingGuideSourceExcelFile
    {
        public StarRisingVotingGuideSourceExcelFile(NominationList nominationList)
            : base(nominationList?.StarRisingNominees) { }
    }
}
