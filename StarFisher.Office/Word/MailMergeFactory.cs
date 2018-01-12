using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    public interface IMailMergeFactory
    {
        IMailMerge GetStarValuesVotingGuideMailMerge(NominationList nominationList);

        IMailMerge GetStarValuesWinnersMemoMailMerge(AwardWinnerList awardWinnerList);
    }

    public class MailMergeFactory : IMailMergeFactory
    {
        private readonly IExcelFileFactory _excelFileFactory;

        public MailMergeFactory(IExcelFileFactory excelFileFactory)
        {
            _excelFileFactory = excelFileFactory;
        }

        public IMailMerge GetStarValuesVotingGuideMailMerge(NominationList nominationList)
        {
            return new StarValuesVotingGuideMailMerge(_excelFileFactory, nominationList);
        }

        public IMailMerge GetStarValuesWinnersMemoMailMerge(AwardWinnerList awardWinnerList)
        {
            return new StarValuesWinnersMemoMailMerge(_excelFileFactory, awardWinnerList);
        }
    }
}
