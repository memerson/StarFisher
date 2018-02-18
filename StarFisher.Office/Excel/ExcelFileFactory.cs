using System;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Excel
{
    public interface IExcelFileFactory
    {
        IExcelFile GetVotingGuideSourceExcelFile(AwardType awardType, NominationList nominationList);

        IExcelFile GetVotingKeyExcelFile(AwardType awardType, NominationList nominationList);

        IExcelFile GetStarValuesNominationNotificationEmailSourceExcelFile(NominationList nominationList);

        IExcelFile GetStarValuesWinnersMemoSourceExcelFile(AwardWinnerList awardWinnerList);

        IExcelFile GetCertificatesSourceExcelFile(AwardType awardType, AwardWinnerList awardWinnerList);

        IExcelFile GetRisingStarNominationNotificationEmailSourceExcelFile(NominationList nominationList);

        IExcelFile GetAwardsLuncheonInviteeListExcelFile(NominationList nominationList);
    }

    public class ExcelFileFactory : IExcelFileFactory
    {
        public IExcelFile GetVotingGuideSourceExcelFile(AwardType awardType, NominationList nominationList)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            if (awardType == AwardType.StarValues)
                return new StarValuesVotingGuideSourceExcelFile(nominationList);
            if (awardType == AwardType.RisingStar)
                return new RisingStarVotingGuideSourceExcelFile(nominationList);

            throw new NotSupportedException(awardType.Value);
        }

        public IExcelFile GetVotingKeyExcelFile(AwardType awardType, NominationList nominationList)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            if (awardType == AwardType.StarValues)
                return new StarValuesVotingKeyExcelFile(nominationList);
            if (awardType == AwardType.RisingStar)
                return new RisingStarVotingKeyExcelFile(nominationList);

            throw new NotSupportedException(awardType.Value);
        }

        public IExcelFile GetStarValuesNominationNotificationEmailSourceExcelFile(NominationList nominationList)
        {
            return new StarValuesNominationNotificationEmailSourceExcelFile(nominationList);
        }

        public IExcelFile GetStarValuesWinnersMemoSourceExcelFile(AwardWinnerList awardWinnerList)
        {
            return new StarValuesWinnersMemoSourceExcelFile(awardWinnerList);
        }

        public IExcelFile GetCertificatesSourceExcelFile(AwardType awardType, AwardWinnerList awardWinnerList)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            if (awardType == AwardType.StarValues)
                return new StarValuesCertificatesSourceExcelFile(awardWinnerList);
            if (awardType == AwardType.RisingStar)
                return new RisingStarCertificatesSourceExcelFile(awardWinnerList);

            throw new NotSupportedException(awardType.Value);
        }

        public IExcelFile GetRisingStarNominationNotificationEmailSourceExcelFile(NominationList nominationList)
        {
            return new RisingStarNominationNotificationEmailSourceExcelFile(nominationList);
        }

        public IExcelFile GetAwardsLuncheonInviteeListExcelFile(NominationList nominationList)
        {
            return new AwardsLuncheonInviteeListExcelFile(nominationList);
        }
    }
}
