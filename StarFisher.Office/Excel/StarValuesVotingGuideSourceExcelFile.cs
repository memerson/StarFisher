using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesVotingGuideSourceExcelFile : VotingGuideSourceExcelFileBase
    {
        public StarValuesVotingGuideSourceExcelFile(NominationList nominationList)
            : base(nominationList?.StarValuesNominations) { }
    }
}
