using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class NomineeVotingIdentifier : ValueObject<NomineeVotingIdentifier>
    {
        public static readonly NomineeVotingIdentifier Unknown = new NomineeVotingIdentifier(new[] { 0 });

        private NomineeVotingIdentifier(ICollection<int> nominationIds)
        {
            NominationIds = new List<int>(nominationIds);

            var items = nominationIds
                .Distinct()
                .Select((id, index) => new {Id = id, Group = id - index})
                .GroupBy(item => item.Group)
                .Select(group =>
                {
                    if (group.Count() >= 3)
                        return $@"{group.First().Id} - {group.Last().Id}";

                    return string.Join(@", ", group.Select(g => g.Id));
                });

            Value = string.Join(", ", items);
        }

        internal static NomineeVotingIdentifier Create(ICollection<int> nomineeIds)
        {
            if (nomineeIds == null || nomineeIds.Count == 0)
                return Unknown;

            return new NomineeVotingIdentifier(nomineeIds);
        }

        internal IReadOnlyCollection<int> NominationIds { get; }

        public string Value { get; }

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
