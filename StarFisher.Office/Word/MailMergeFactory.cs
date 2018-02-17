using System;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    public interface IMailMergeFactory
    {
        IMailMerge GetVotingGuideMailMerge(AwardType awardType, NominationList nominationList);

        IMailMerge GetStarValuesWinnersMemoMailMerge(AwardWinnerList awardWinnerList);
    }

    public class MailMergeFactory : IMailMergeFactory
    {
        private readonly IExcelFileFactory _excelFileFactory;

        public MailMergeFactory(IExcelFileFactory excelFileFactory)
        {
            _excelFileFactory = excelFileFactory;
        }

        public IMailMerge GetVotingGuideMailMerge(AwardType awardType, NominationList nominationList)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            if (awardType == AwardType.StarValues)
                return new StarValuesVotingGuideMailMerge(_excelFileFactory, nominationList);
            if(awardType == AwardType.RisingStar)
                return new RisingStarVotingGuideMailMerge(_excelFileFactory, nominationList);

            throw new NotSupportedException(awardType.Value);
        }

        public IMailMerge GetStarValuesWinnersMemoMailMerge(AwardWinnerList awardWinnerList)
        {
            return new StarValuesWinnersMemoMailMerge(_excelFileFactory, awardWinnerList);
        }
    }
}
