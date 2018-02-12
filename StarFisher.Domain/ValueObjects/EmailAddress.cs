using System;
using System.Text.RegularExpressions;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class EmailAddress : ValueObject<EmailAddress>
    {
        public static readonly EmailAddress Invalid = new EmailAddress(@"INVALID");

        public static readonly EmailAddress None = new EmailAddress(string.Empty);

        private const string EmailAddressFromNameFormat = @"{0}.{1}@healthstream.com";

        private static readonly Regex ValidationRegex = new Regex(@"^\w+([-+.']\w+)*@healthstream\.com$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private EmailAddress(string emailAddressText)
        {
            Value = emailAddressText ?? string.Empty;
        }

        public static EmailAddress Create(string emailAddressText)
        {
            return new EmailAddress(emailAddressText);
        }

        internal static EmailAddress Create(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                return Invalid;

            return new EmailAddress(string.Format(EmailAddressFromNameFormat, firstName, lastName));
        }

        public static bool GetIsValid(string emailAddressText)
        {
            return !string.IsNullOrWhiteSpace(emailAddressText) && ValidationRegex.IsMatch(emailAddressText);
        }

        public string Value { get; }

        public bool IsValid => GetIsValid(Value);

        protected override bool EqualsCore(EmailAddress other)
        {
            return string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override int GetHashCodeCore()
        {
            return Value.ToLower().GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
