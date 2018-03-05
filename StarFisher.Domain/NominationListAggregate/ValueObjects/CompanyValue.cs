using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class CompanyValue : ValueObject<CompanyValue>
    {
        public static readonly CompanyValue LearningCulture = new CompanyValue(@"Learning Culture");
        public static readonly CompanyValue Innovation = new CompanyValue(@"Innovation");
        public static readonly CompanyValue CustomerFocus = new CompanyValue(@"Customer Focus");
        public static readonly CompanyValue IndividualIntegrity = new CompanyValue(@"Individual Integrity");
        public static readonly CompanyValue Performance = new CompanyValue(@"Performance");

        private static readonly List<CompanyValue> ValidCompanyValues = new List<CompanyValue>
        {
            LearningCulture,
            Innovation,
            CustomerFocus,
            IndividualIntegrity,
            Performance
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