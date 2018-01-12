using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesVotingGuideSourceExcelFile : VotingGuideSourceExcelFile
    {
        public StarValuesVotingGuideSourceExcelFile(NominationList nominationList)
            : base(nominationList?.StarValuesNominees) { }
    }
}
