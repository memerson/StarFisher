using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    public interface IExcelFileFactory
    {
        IExcelFile GetStarValuesVotingGuideSourceExcelFile(NominationList nominationList);

        IExcelFile GetStarValuesVotingKeyExcelFile(NominationList nominationList);

        IExcelFile GetStarValuesNominationNotificationEmailSourceExcelFile(NominationList nominationList);

        IExcelFile GetStarValuesWinnersMemoSourceExcelFile(AwardWinnerList awardWinnerList);

        IExcelFile GetStarRisingVotingGuideSourceExcelFile(NominationList nominationList);

        IExcelFile GetStarRisingVotingKeyExcelFile(NominationList nominationList);

        IExcelFile GetStarRisingNominationNotificationEmailSourceExcelFile(NominationList nominationList);
    }

    public class ExcelFileFactory : IExcelFileFactory
    {
        public IExcelFile GetStarValuesVotingGuideSourceExcelFile(NominationList nominationList)
        {
            return new StarValuesVotingGuideSourceExcelFile(nominationList);
        }

        public IExcelFile GetStarValuesVotingKeyExcelFile(NominationList nominationList)
        {
            return new StarValuesVotingKeyExcelFile(nominationList);
        }

        public IExcelFile GetStarValuesNominationNotificationEmailSourceExcelFile(NominationList nominationList)
        {
            return new StarValuesNominationNotificationEmailSourceExcelFile(nominationList);
        }

        public IExcelFile GetStarValuesWinnersMemoSourceExcelFile(AwardWinnerList awardWinnerList)
        {
            return new StarValuesWinnersMemoSourceExcelFile(awardWinnerList);
        }

        public IExcelFile GetStarRisingVotingGuideSourceExcelFile(NominationList nominationList)
        {
            return new StarRisingVotingGuideSourceExcelFile(nominationList);
        }

        public IExcelFile GetStarRisingVotingKeyExcelFile(NominationList nominationList)
        {
            return new StarRisingVotingKeyExcelFile(nominationList);
        }

        public IExcelFile GetStarRisingNominationNotificationEmailSourceExcelFile(NominationList nominationList)
        {
            return new StarRisingNominationNotificationEmailSourceExcelFile(nominationList);
        }
    }
}
