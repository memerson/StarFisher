using StarFisher.Office.Excel;

namespace StarFisher.Office.Word.MailMergeTemplates
{
    public interface IMailMergeFactory
    {
        IMailMerge GetStarValuesVotingGuideMailMerge();
    }

    public class MailMergeFactory : IMailMergeFactory
    {
        private readonly IExcelFileFactory _excelFileFactory;

        public MailMergeFactory(IExcelFileFactory excelFileFactory)
        {
            _excelFileFactory = excelFileFactory;
        }

        public IMailMerge GetStarValuesVotingGuideMailMerge()
        {
            return new StarValuesVotingGuideMailMerge(_excelFileFactory);
        }
    }
}
