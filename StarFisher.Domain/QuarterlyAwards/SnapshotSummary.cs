using System;

namespace StarFisher.Domain.QuarterlyAwards
{
    public class SnapshotSummary : IComparable<SnapshotSummary>
    {
        internal SnapshotSummary(DateTime dateTime, string lastChangeSummary)
        {
            if (string.IsNullOrWhiteSpace(lastChangeSummary))
                lastChangeSummary = @"Unknown change";

            DateTime = dateTime;
            LastChangeSummary = lastChangeSummary;
        }

        public DateTime DateTime { get; }

        public string LastChangeSummary { get; }

        public int CompareTo(SnapshotSummary other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            return ReferenceEquals(null, other) ? 1 : DateTime.CompareTo(other.DateTime);
        }
    }
}