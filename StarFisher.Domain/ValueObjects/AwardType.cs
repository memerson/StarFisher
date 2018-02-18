using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class AwardType : ValueObject<AwardType>
    {
        public static readonly AwardType StarValues = new AwardType(
            @"Employee - Star Values Award Nominee", @"StarValues", @"Star Values Award", true);
        public static readonly AwardType RisingStar = new AwardType(
            @"Intern - Rising Star Award Nominee", @"RisingStar", @"Rising Star Award", true);

        private static readonly List<AwardType> ValidAwardTypes = new List<AwardType>
        {
            StarValues,
            RisingStar
        };

        public static readonly AwardType Invalid = new AwardType(@"INVALID", @"INVALID", @"INVALID", false);

        private AwardType(string value, string fileNameIdentifier, string prettyName, bool supportsVoting)
        {
            Value = value;
            SupportsVoting = supportsVoting;
            FileNameIdentifier = fileNameIdentifier;
            PrettyName = prettyName;
        }

        internal static AwardType Create(string awardType)
        {
            if (string.IsNullOrWhiteSpace(awardType))
                return Invalid;

            return ValidAwardTypes.FirstOrDefault(et => awardType.StartsWith(et.Value)) ?? Invalid;
        }

        public string Value { get; }

        public string PrettyName { get; }

        public bool SupportsVoting { get; }

        public string GetVotingGuideFileName(Year year, Quarter quarter)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));
            if (!SupportsVoting)
                throw new InvalidOperationException(@"This award type does not support voting.");

            return $@"{year}{quarter.Abbreviation}_{FileNameIdentifier}_VotingGuide.docx";
        }

        public string GetVotingKeyFileName(Year year, Quarter quarter)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));
            if (!SupportsVoting)
                throw new InvalidOperationException(@"This award type does not support voting.");

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

        private string FileNameIdentifier { get; }
    }
}
