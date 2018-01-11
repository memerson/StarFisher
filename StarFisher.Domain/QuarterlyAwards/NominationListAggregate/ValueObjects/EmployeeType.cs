using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate.ValueObjects
{
    public class EmployeeType : ValueObject<EmployeeType>
    {
        public static readonly EmployeeType Employee = new EmployeeType(@"Employee - Star Values Award Nominee");
        public static readonly EmployeeType Intern = new EmployeeType(@"Intern - Rising Star Award Nominee");

        private static readonly List<EmployeeType> ValidEmployeeTypes = new List<EmployeeType>
        {
            Employee,
            Intern
        };

        public static readonly EmployeeType Invalid = new EmployeeType(@"INVALID");

        private EmployeeType(string value)
        {
            Value = value;
        }

        public static EmployeeType Create(string employeeTypeText)
        {
            if (string.IsNullOrWhiteSpace(employeeTypeText))
                return Invalid;

            var employeeType = ValidEmployeeTypes.FirstOrDefault(et => employeeTypeText.StartsWith(et.Value));
            return employeeType ?? Invalid;
        }

        public string Value { get; }

        protected override bool EqualsCore(EmployeeType other)
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
