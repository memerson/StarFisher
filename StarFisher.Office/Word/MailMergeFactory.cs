using System;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    public interface IMailMergeFactory
    {
        IMailMerge GetVotingGuideMailMerge(AwardType awardType, NominationList nominationList);
        IMailMerge GetStarValuesWinnersMemoMailMerge(NominationList nominationList);
        IMailMerge GetRisingStarWinnersMemoMailMerge(NominationList nominationList);
        IMailMerge GetCertificatesMailMerge(AwardType awardType, NominationList nominationList);
        IMailMerge GetNominationNotificationsMailMerge(AwardType awardType, NominationList nominationList);
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
            if (awardType == AwardType.RisingStar)
                return new RisingStarVotingGuideMailMerge(_excelFileFactory, nominationList);

            throw new NotSupportedException(awardType.Value);
        }

        public IMailMerge GetStarValuesWinnersMemoMailMerge(NominationList nominationList)
        {
            return new StarValuesWinnersMemoMailMerge(_excelFileFactory, nominationList);
        }

        public IMailMerge GetRisingStarWinnersMemoMailMerge(NominationList nominationList)
        {
            return new RisingStarWinnersMemoMailMerge(_excelFileFactory, nominationList);
        }

        public IMailMerge GetCertificatesMailMerge(AwardType awardType, NominationList nominationList)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            if (awardType == AwardType.StarValues)
                return new StarValuesCertificatesMailMerge(_excelFileFactory, nominationList);
            if (awardType == AwardType.RisingStar)
                return new RisingStarCertificatesMailMerge(_excelFileFactory, nominationList);

            throw new NotSupportedException(awardType.Value);
        }

        public IMailMerge GetNominationNotificationsMailMerge(AwardType awardType, NominationList nominationList)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            if (awardType == AwardType.StarValues)
                return new StarValuesNominationNotificationsMailMerge(_excelFileFactory, nominationList);

            throw new NotSupportedException(awardType.Value);
        }
    }
}