using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    public class StarValuesVotingGuideMailMerge : MailMergeBase
    {
        private readonly IExcelFileFactory _excelFileFactory;

        public StarValuesVotingGuideMailMerge(IExcelFileFactory excelFileFactory)
            : base(@"StarFisher.Office.Word.MailMergeTemplates.StarValuesVotingGuideMailMergeTemplate.docx")
        {
            _excelFileFactory = excelFileFactory;
        }

        protected override IExcelFile GetDataSourceExcelFile(NominationList nominationList)
        {
            return _excelFileFactory.GetStarValuesVotingGuideSourceExcelFile(nominationList);
        }
    }
}