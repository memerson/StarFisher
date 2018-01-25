using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarRisingVotingGuideSourceExcelFile : VotingGuideSourceExcelFileBase
    {
        public StarRisingVotingGuideSourceExcelFile(NominationList nominationList)
            : base(nominationList?.StarRisingNominees) { }
    }
}
