﻿using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class EmailAddress : ValueObject<EmailAddress>
    {
        public static readonly EmailAddress Invalid = new EmailAddress(@"INVALID");

        public static readonly EmailAddress None = new EmailAddress(string.Empty);

        private const string EmailAddressFromNameFormat = @"{0}.{1}@healthstream.com";

        private EmailAddress(string emailAddressText)
        {
            Value = emailAddressText;
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

        public string Value { get; }

        protected override bool EqualsCore(EmailAddress other)
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
