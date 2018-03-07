using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class NominationWriteUpSummary : ValueObject<NominationWriteUpSummary>
    {
        public static readonly NominationWriteUpSummary Invalid = new NominationWriteUpSummary(@"INVALID");
        public static readonly NominationWriteUpSummary NotApplicable = new NominationWriteUpSummary(@"N/A");

        private NominationWriteUpSummary(string value)
        {
            Value = value;
        }

        public string Value { get; }

        internal static NominationWriteUpSummary Create(string nominationWriteUpSummaryText)
        {
            if (string.IsNullOrWhiteSpace(nominationWriteUpSummaryText))
                return Invalid;

            if (string.Equals(nominationWriteUpSummaryText, NotApplicable.Value))
                return NotApplicable;

            return new NominationWriteUpSummary(nominationWriteUpSummaryText);
        }

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