using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class NomineeVotingIdentifier : ValueObject<NomineeVotingIdentifier>
    {
        public static readonly NomineeVotingIdentifier Unknown = new NomineeVotingIdentifier(new[] {-1});

        private NomineeVotingIdentifier(IEnumerable<int> nominationIndexes)
        {
            var items = nominationIndexes
                .Distinct()
                .Select((index, metaIndex) => new {Value = index + 1, Group = index - metaIndex})
                .GroupBy(item => item.Group)
                .Select(group =>
                {
                    if (group.Count() >= 3)
                        return $@"{group.First().Value} - {group.Last().Value}";

                    return string.Join(@", ", group.Select(g => g.Value));
                });

            Value = string.Join(", ", items);
        }

        public string Value { get; }

        internal static NomineeVotingIdentifier Create(ICollection<int> nomineeIndexes)
        {
            if (nomineeIndexes == null || nomineeIndexes.Count == 0)
                return Unknown;

            return new NomineeVotingIdentifier(nomineeIndexes);
        }

        protected override bool EqualsCore(NomineeVotingIdentifier other)
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