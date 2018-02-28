using System;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Office.Excel
{
    public interface IExcelFileFactory
    {
        IExcelFile GetVotingGuideSourceExcelFile(AwardType awardType, NominationList nominationList);
        IExcelFile GetVotingKeyExcelFile(AwardType awardType, NominationList nominationList);
        IExcelFile GetNominationNotificationEmailSourceExcelFile(AwardType awardType, NominationList nominationList);
        IExcelFile GetStarValuesWinnersMemoSourceExcelFile(NominationList nominationList);
        IExcelFile GetRisingStarWinnersMemoSourceExcelFile(NominationList nominationList);
        IExcelFile GetCertificatesSourceExcelFile(AwardType awardType, NominationList nominationList);
        IExcelFile GetAwardsLuncheonInviteeListExcelFile(NominationList nominationList);
        IExcelFile GetStarValuesNomineeListExcelFile(NominationList nominationList);
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

        public IExcelFile GetNominationNotificationEmailSourceExcelFile(AwardType awardType, NominationList nominationList)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            if (awardType == AwardType.StarValues)
                return new StarValuesNominationNotificationEmailSourceExcelFile(nominationList);
            if (awardType == AwardType.RisingStar)
                return new RisingStarNominationNotificationEmailSourceExcelFile(nominationList);

            throw new NotSupportedException(awardType.Value);
        }

        public IExcelFile GetStarValuesWinnersMemoSourceExcelFile(NominationList nominationList)
        {
            return new StarValuesWinnersMemoSourceExcelFile(nominationList);
        }

        public IExcelFile GetRisingStarWinnersMemoSourceExcelFile(NominationList nominationList)
        {
            return new RisingStarWinnersMemoSourceExcelFile(nominationList);
        }

        public IExcelFile GetCertificatesSourceExcelFile(AwardType awardType, NominationList nominationList)
        {
            if (awardType == null)
                throw new ArgumentNullException(nameof(awardType));

            if (awardType == AwardType.StarValues)
                return new StarValuesCertificatesSourceExcelFile(nominationList);
            if (awardType == AwardType.RisingStar)
                return new RisingStarCertificatesSourceExcelFile(nominationList);

            throw new NotSupportedException(awardType.Value);
        }

        public IExcelFile GetAwardsLuncheonInviteeListExcelFile(NominationList nominationList)
        {
            return new AwardsLuncheonInviteeListExcelFile(nominationList);
        }

        public IExcelFile GetStarValuesNomineeListExcelFile(NominationList nominationList)
        {
            return new StarValuesNomineeListExcelFile(nominationList);
        }
    }
}