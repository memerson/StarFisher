
namespace StarFisher.Domain.Common
{
    public abstract class AggregateRoot : Entity
    {
        protected AggregateRoot(int id)
            : base(id)
        { }

        internal bool IsDirty { get; private set; }

        internal void SetCurrent() => IsDirty = false;

        protected void MarkAsDirty() => IsDirty = true;
    }
}
