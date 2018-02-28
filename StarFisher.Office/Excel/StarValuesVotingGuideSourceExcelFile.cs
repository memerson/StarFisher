using StarFisher.Domain.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesVotingGuideSourceExcelFile : VotingGuideSourceExcelFileBase
    {
        public StarValuesVotingGuideSourceExcelFile(NominationList nominationList)
            : base(nominationList?.StarValuesNominations)
        {
        }
    }
}