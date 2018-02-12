
using System;

namespace StarFisher.Domain.Common
{
    public abstract class AggregateRoot : Entity
    {
        protected AggregateRoot(int id)
            : base(id)
        {
            LastChangeSummary = @"Unmodified";
        }

        internal string LastChangeSummary { get; private set; }

        internal bool IsDirty { get; private set; }

        internal void SetCurrent() => IsDirty = false;

        protected void MarkAsDirty(string changeSummary)
        {
            if(string.IsNullOrWhiteSpace(changeSummary))
                throw new ArgumentException(nameof(changeSummary));
            
            IsDirty = true;
            LastChangeSummary = changeSummary;
        }
    }
}
