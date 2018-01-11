
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class NominationWriteUpSummary : ValueObject<NominationWriteUpSummary>
    {
        public static readonly NominationWriteUpSummary Invalid = new NominationWriteUpSummary(@"INVALID");

        private NominationWriteUpSummary(string value)
        {
            Value = value;
        }

        internal static NominationWriteUpSummary Create(string nominationWriteUpSummaryText)
        {
            if (string.IsNullOrWhiteSpace(nominationWriteUpSummaryText))
                return Invalid;

            return new NominationWriteUpSummary(nominationWriteUpSummaryText);
        }

        public string Value { get; }
        protected override bool EqualsCore(NominationWriteUpSummary other)
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
