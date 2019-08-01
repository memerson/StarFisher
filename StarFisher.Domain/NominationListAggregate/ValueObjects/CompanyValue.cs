using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class CompanyValue : ValueObject<CompanyValue>
    {
        public static readonly CompanyValue ContinuouslyImproving = new CompanyValue(@"Continuously Improving");
        public static readonly CompanyValue DrivingInnovation = new CompanyValue(@"Driving Innovation");
        public static readonly CompanyValue DelightingCustomers = new CompanyValue(@"Delighting Customers");
        public static readonly CompanyValue BehavingWithIntegrity = new CompanyValue(@"Behaving with Integrity");
        public static readonly CompanyValue DeliveringMeaningfulOutcomes = new CompanyValue(@"Delivering Meaningful Outcomes");
        public static readonly CompanyValue StreamingGood = new CompanyValue(@"Streaming Good");

        private static readonly List<CompanyValue> ValidCompanyValues = new List<CompanyValue>
        {
            ContinuouslyImproving,
            DrivingInnovation,
            DelightingCustomers,
            BehavingWithIntegrity,
            DeliveringMeaningfulOutcomes,
            StreamingGood
        };

        public static readonly CompanyValue Invalid = new CompanyValue(@"INVALID");

        private CompanyValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        internal static CompanyValue FindByValue(string companyValueText)
        {
            var companyValue = ValidCompanyValues.FirstOrDefault(cv => cv.ToString() == companyValueText);
            return companyValue ?? Invalid;
        }

        protected override bool EqualsCore(CompanyValue other)
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