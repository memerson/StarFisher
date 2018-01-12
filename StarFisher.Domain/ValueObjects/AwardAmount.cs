using System;

namespace StarFisher.Domain.ValueObjects
{
    public class AwardAmount
    {
        private AwardAmount(decimal valueInDollars)
        {
            ValueInDollars = valueInDollars;
        }

        public static AwardAmount Create(int valueInDollars)
        {
            if(valueInDollars <= 0)
                throw new ArgumentOutOfRangeException(nameof(valueInDollars));

            return new AwardAmount((decimal)valueInDollars);
        }

        public decimal ValueInDollars { get; }
    }
}
