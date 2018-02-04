
using System;

namespace StarFisher.Domain.ValueObjects
{
    public class NominationWriteUp
    {
        private NominationWriteUp(string value, bool containsNomineeName)
        {
            Value = value;
            ContainsNomineeName = containsNomineeName;
        }

        public static NominationWriteUp Create(PersonName nomineeName, string nominationWriteUpText)
        {
            if(nomineeName == null)
                throw new ArgumentNullException(nameof(nomineeName));

            var value = nominationWriteUpText ?? string.Empty;
            var containsNomineeName = GetContainsNomineeName(nomineeName, nominationWriteUpText);

            return new NominationWriteUp(value, containsNomineeName);
        }

        public static bool GetIsValid(PersonName nomineeName, string nominationWriteUpText)
        {
            return !string.IsNullOrWhiteSpace(nominationWriteUpText) &&
                   !GetContainsNomineeName(nomineeName, nominationWriteUpText);
        }

        public bool IsValid => !ContainsNomineeName && !string.IsNullOrWhiteSpace(Value);

        public string Value { get; }

        public bool ContainsNomineeName { get; }

        public override string ToString()
        {
            return Value;
        }

        private static bool GetContainsNomineeName(PersonName nomineeName, string nominationWriteUpText)
        {
            var value = nominationWriteUpText ?? string.Empty;
            return value.Contains(nomineeName.FirstName) || value.Contains(nomineeName.LastName);
        }
    }
}
