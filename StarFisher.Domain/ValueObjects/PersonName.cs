using System;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class PersonName : ValueObject<PersonName>
    {
        private PersonName(string nameText, bool isAnonymous)
        {
            // TODO: Account for invalid names.

            RawNameText = nameText ?? string.Empty;
            IsAnonymous = isAnonymous;
            FullName = string.Empty;
            FullNameLastNameFirst = string.Empty;
            DerivedEmailAddress = EmailAddress.Invalid;

            if (isAnonymous)
            {
                FullName = FullNameLastNameFirst = FirstName = LastName = @"Anonymous";
                return;
            }

            var nameParts = RawNameText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            switch (nameParts.Length)
            {
                case 2:
                    FirstName = nameParts[0];
                    LastName = nameParts[1];
                    FullName = $@"{FirstName} {LastName}";
                    FullNameLastNameFirst = $@"{LastName}, {FirstName}";
                    DerivedEmailAddress = EmailAddress.Create(FirstName, LastName);
                    break;
                case 3:
                    FirstName = nameParts[0];
                    LastName = nameParts[2];
                    FullName = $@"{FirstName} {nameParts[1]} {LastName}";
                    FullNameLastNameFirst = $@"{LastName}, {FirstName} {nameParts[1]}";
                    DerivedEmailAddress = EmailAddress.Create(FirstName, LastName);
                    break;
            }
        }

        public string FirstName { get; }

        public string LastName { get; }

        public string RawNameText { get; }

        public bool IsAnonymous { get; }

        public string FullName { get; }

        public string FullNameLastNameFirst { get; }

        public EmailAddress DerivedEmailAddress { get; }

        public static PersonName Create(string nameText)
        {
            return new PersonName(nameText, false);
        }

        internal static PersonName CreateForNominator(string nameText, bool isAnonymous)
        {
            return new PersonName(nameText, isAnonymous);
        }

        public static bool IsValid(string nameText)
        {
            if (string.IsNullOrWhiteSpace(nameText))
                return false;

            var nameParts = nameText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length < 2 || nameParts.Length > 3)
                return false;

            return true;
        }

        protected override bool EqualsCore(PersonName other)
        {
            return string.Equals(RawNameText, other.RawNameText);
        }

        protected override int GetHashCodeCore()
        {
            return RawNameText.GetHashCode();
        }

        public override string ToString()
        {
            return RawNameText;
        }
    }
}
