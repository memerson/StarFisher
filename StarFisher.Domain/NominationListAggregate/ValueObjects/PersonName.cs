using System;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class PersonName : ValueObject<PersonName>
    {
        private PersonName(string nameText, bool isAnonymous)
        {
            RawNameText = nameText?.Trim() ?? string.Empty;
            IsAnonymous = isAnonymous;
            FullName = string.Empty;
            FullNameLastNameFirst = string.Empty;
            DerivedEmailAddress = EmailAddress.Invalid;

            if (isAnonymous)
            {
                FullName = FullNameLastNameFirst = FirstName = LastName = @"Anonymous";
                return;
            }

            var nameParts = RawNameText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            switch (nameParts.Length)
            {
                case 0:
                    FirstName = @"UNKNOWN";
                    LastName = @"UNKNOWN";
                    FullNameLastNameFirst = FullName = @"UNKNOWN NAME";
                    DerivedEmailAddress = EmailAddress.Invalid;
                    break;
                case 1:
                    FirstName = @"UNKNOWN";
                    LastName = nameParts[0];
                    FullName = LastName;
                    FullNameLastNameFirst = LastName;
                    DerivedEmailAddress = EmailAddress.Invalid;
                    break;
                case 3:
                    FirstName = nameParts[0];
                    var middleName = nameParts[1];
                    LastName = nameParts[2];
                    FullName = $@"{FirstName} {middleName} {LastName}";
                    FullNameLastNameFirst = $@"{LastName}, {FirstName} {middleName}";
                    DerivedEmailAddress = EmailAddress.Create(FirstName, LastName);
                    break;
                default: // 2 name parts or more than 3; in either case we can't tell the middle name.
                    FirstName = nameParts[0];
                    LastName = nameParts[nameParts.Length - 1];
                    FullName = $@"{FirstName} {LastName}";
                    FullNameLastNameFirst = $@"{LastName}, {FirstName}";
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

        public bool IsValid => GetIsValid(RawNameText);

        public EmailAddress DerivedEmailAddress { get; }

        public static PersonName Create(string nameText)
        {
            return new PersonName(nameText, false);
        }

        internal static PersonName CreateForNominator(string nameText, bool isAnonymous)
        {
            return new PersonName(nameText, isAnonymous);
        }

        public static bool GetIsValid(string nameText)
        {
            if (string.IsNullOrWhiteSpace(nameText))
                return false;

            var nameParts = nameText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            return nameParts.Length >= 2 && nameParts.Length <= 3;
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