using System.Collections.Generic;
using System.Linq;

namespace StarFisher.Domain.ValueObjects
{
    public class AwardAmount
    {
        public static readonly AwardAmount StarPerformanceFullTimeFirstPlace = new AwardAmount(300);
        public static readonly AwardAmount StarPerformanceFullTimeSecondPlace = new AwardAmount(200);
        public static readonly AwardAmount StarPerformanceFullTimeThirdPlace = new AwardAmount(100);

        public static readonly AwardAmount StarPerformancePartTimeFirstPlace = new AwardAmount(150);
        public static readonly AwardAmount StarPerformancePartTimeSecondPlace = new AwardAmount(100);
        public static readonly AwardAmount StarPerformancePartTimeThirdPlace = new AwardAmount(50);

        public static readonly AwardAmount RisingPerformanceFullTime = new AwardAmount(100);
        public static readonly AwardAmount RisingPerformancePartTime = new AwardAmount(50);

        public static readonly AwardAmount StarValues = new AwardAmount(350);
        public static readonly AwardAmount StarRising = new AwardAmount(100);

        public static readonly AwardAmount Invalid = new AwardAmount(0);

        private static readonly List<AwardAmount> ValidAwardAmounts = new List<AwardAmount>
        {
            StarPerformanceFullTimeFirstPlace,
            StarPerformanceFullTimeSecondPlace,
            StarPerformanceFullTimeThirdPlace,
            StarPerformancePartTimeFirstPlace,
            StarPerformancePartTimeSecondPlace,
            StarPerformancePartTimeThirdPlace,
            RisingPerformanceFullTime,
            RisingPerformancePartTime,
            StarValues,
            StarRising,
        };

        private AwardAmount(decimal valueInDollars)
        {
            ValueInDollars = valueInDollars;
        }

        internal static AwardAmount Create(decimal valueInDollars)
        {
            return ValidAwardAmounts.FirstOrDefault(aa => aa.ValueInDollars == valueInDollars) ?? Invalid;
        }

        public decimal ValueInDollars { get; }
    }
}
