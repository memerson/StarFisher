using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class AwardType : ValueObject<AwardType>
    {
        public static readonly AwardType StarValues = new AwardType(@"Employee - Star Values Award Nominee");
        public static readonly AwardType StarRising = new AwardType(@"Intern - Rising Star Award Nominee");
        public static readonly AwardType StarPerformance = new AwardType(@"Star Performance");
        public static readonly AwardType RisingPerformance = new AwardType(@"Rising Performance");

        private static readonly List<AwardType> ValidAwardTypes = new List<AwardType>
        {
            StarValues,
            StarRising, 
            StarPerformance,
            RisingPerformance
        };

        public static readonly AwardType Invalid = new AwardType(@"INVALID");

        private AwardType(string value)
        {
            Value = value;
        }

        public static AwardType Create(string awardType)
        {
            if (string.IsNullOrWhiteSpace(awardType))
                return Invalid;

            return ValidAwardTypes.FirstOrDefault(et => awardType.StartsWith(et.Value)) ?? Invalid;
        }

        public string Value { get; }

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
