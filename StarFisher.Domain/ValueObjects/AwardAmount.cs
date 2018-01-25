using System;

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

        private AwardAmount(decimal valueInDollars)
        {
            ValueInDollars = valueInDollars;
        }

        public decimal ValueInDollars { get; }
    }
}
