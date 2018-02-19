using System.Collections.Generic;
using System.Linq;

namespace StarFisher.Domain.ValueObjects
{
    public class AwardAmount
    {
        public static readonly AwardAmount StarValues = new AwardAmount(350);
        public static readonly AwardAmount RisingStar = new AwardAmount(100);

        public static readonly AwardAmount Invalid = new AwardAmount(0);

        private static readonly List<AwardAmount> ValidAwardAmounts = new List<AwardAmount>
        {
            StarValues,
            RisingStar
        };

        private AwardAmount(decimal valueInDollars)
        {
            ValueInDollars = valueInDollars;
        }

        public decimal ValueInDollars { get; }

        internal static AwardAmount Create(decimal valueInDollars)
        {
            return ValidAwardAmounts.FirstOrDefault(aa => aa.ValueInDollars == valueInDollars) ?? Invalid;
        }
    }
}