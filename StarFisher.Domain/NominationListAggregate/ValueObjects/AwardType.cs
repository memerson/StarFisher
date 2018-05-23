using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class AwardType : ValueObject<AwardType>
    {
        public static readonly AwardType StarValues = new AwardType(@"Employee - Star Values Award Nominee",
            @"StarValues", @"Star Values Award", AwardAmount.StarValues, AwardCategory.QuarterlyAwards);

        public static readonly AwardType RisingStar = new AwardType(@"Intern - Rising Star Award Nominee",
            @"RisingStar", @"Rising Star Award", AwardAmount.RisingStar, AwardCategory.QuarterlyAwards);

        public static readonly AwardType SuperStar = new AwardType(@"Employee - Super Star Award Nominee", @"SuperStar",
            @"Super Star Award", AwardAmount.SuperStar, AwardCategory.SuperStarAwards);

        public static readonly AwardType Invalid =
            new AwardType(@"INVALID", @"INVALID", @"INVALID", AwardAmount.Invalid, AwardCategory.Invalid);

        private AwardType(string value, string fileNameIdentifier, string prettyName, AwardAmount awardAmount,
            AwardCategory awardCategory)
        {
            Value = value;
            PrettyName = prettyName;
            FileNameIdentifier = fileNameIdentifier;
            AwardAmount = awardAmount;
            AwardCategory = awardCategory;
        }

        public static IReadOnlyList<AwardType> ValidAwardTypes { get; } = new List<AwardType>
        {
            StarValues,
            RisingStar,
            SuperStar
        };

        public string Value { get; }

        public string PrettyName { get; }

        public AwardAmount AwardAmount { get; }

        public AwardCategory AwardCategory { get; }

        private string FileNameIdentifier { get; }

        internal static AwardType FindByAwardName(string awardType)
        {
            if (string.IsNullOrWhiteSpace(awardType))
                return Invalid;

            return ValidAwardTypes.FirstOrDefault(et => awardType.StartsWith(et.Value)) ?? Invalid;
        }

        public string GetVotingGuideFileName(AwardsPeriod awardsPeriod)
        {
            if (awardsPeriod == null)
                throw new ArgumentNullException(nameof(awardsPeriod));

            return $@"{awardsPeriod.FileNamePrefix}_{FileNameIdentifier}_VotingGuide.pdf";
        }

        public string GetVotingKeyFileName(AwardsPeriod awardsPeriod)
        {
            if (awardsPeriod == null)
                throw new ArgumentNullException(nameof(awardsPeriod));

            return $@"{awardsPeriod.FileNamePrefix}_{FileNameIdentifier}_VotingKey.xlsx";
        }

        public string GetCertificatesFileName(AwardsPeriod awardsPeriod)
        {
            if (awardsPeriod == null)
                throw new ArgumentNullException(nameof(awardsPeriod));

            return $@"{awardsPeriod.FileNamePrefix}_{FileNameIdentifier}_Certificates.docx";
        }

        public string GetWinnersForMemoFileName(AwardsPeriod awardsPeriod)
        {
            if (awardsPeriod == null)
                throw new ArgumentNullException(nameof(awardsPeriod));

            return $@"{awardsPeriod.FileNamePrefix}_{FileNameIdentifier}_WinnersForMemo.docx";
        }

        public string GetNomineesForMemoFileName(AwardsPeriod awardsPeriod)
        {
            if (awardsPeriod == null)
                throw new ArgumentNullException(nameof(awardsPeriod));

            return $@"{awardsPeriod.FileNamePrefix}_{FileNameIdentifier}_NomineesForMemo.xlsx";
        }

        protected override bool EqualsCore(AwardType other)
        {
            return string.Equals(Value, other.Value);
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}