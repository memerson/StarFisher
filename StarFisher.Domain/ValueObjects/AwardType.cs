using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class AwardType : ValueObject<AwardType>
    {
        public static readonly AwardType StarValues = new AwardType(
            @"Employee - Star Values Award Nominee", @"StarValues", @"Star Values Award", AwardAmount.StarValues);

        public static readonly AwardType RisingStar = new AwardType(
            @"Intern - Rising Star Award Nominee", @"RisingStar", @"Rising Star Award", AwardAmount.RisingStar);

        private static readonly List<AwardType> ValidAwardTypes = new List<AwardType>
        {
            StarValues,
            RisingStar
        };

        public static readonly AwardType Invalid =
            new AwardType(@"INVALID", @"INVALID", @"INVALID", AwardAmount.Invalid);

        private AwardType(string value, string fileNameIdentifier, string prettyName, AwardAmount awardAmount)
        {
            Value = value;
            PrettyName = prettyName;
            FileNameIdentifier = fileNameIdentifier;
            AwardAmount = awardAmount;
        }

        public string Value { get; }

        public string PrettyName { get; }

        public AwardAmount AwardAmount { get; }

        private string FileNameIdentifier { get; }

        internal static AwardType Create(string awardType)
        {
            if (string.IsNullOrWhiteSpace(awardType))
                return Invalid;

            return ValidAwardTypes.FirstOrDefault(et => awardType.StartsWith(et.Value)) ?? Invalid;
        }

        public string GetVotingGuideFileName(Year year, Quarter quarter)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));

            return $@"{year}{quarter.Abbreviation}_{FileNameIdentifier}_VotingGuide.docx";
        }

        public string GetVotingKeyFileName(Year year, Quarter quarter)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));

            return $@"{year}{quarter.Abbreviation}_{FileNameIdentifier}_VotingKey.xlsx";
        }

        public string GetCertificatesFileName(Year year, Quarter quarter)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));

            return $@"{year}{quarter.Abbreviation}_{FileNameIdentifier}_Certificates.docx";
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