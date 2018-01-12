
using System;
using Remotion.Data.Linq.Parsing;

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
            var containsNomineeName = value.Contains(nomineeName.FirstName) || value.Contains(nomineeName.LastName);

            return new NominationWriteUp(value, containsNomineeName);
        }

        public bool IsValid => !ContainsNomineeName && !string.IsNullOrWhiteSpace(Value);

        public string Value { get; }

        public bool ContainsNomineeName { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}
