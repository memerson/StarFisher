using System;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class Year : ValueObject<Year>
    {
        public static Year Invalid = new Year(0);

        private Year(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public static Year Create(int value)
        {
            if (!IsValid(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            return new Year(value);
        }

        public static bool IsValid(int value)
        {
            return value >= 2017 && value <= DateTime.Now.Year;
        }

        protected override bool EqualsCore(Year other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}