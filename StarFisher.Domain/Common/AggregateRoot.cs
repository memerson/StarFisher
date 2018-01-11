
namespace StarFisher.Domain.Common
{
    public abstract class AggregateRoot : Entity
    {
        protected AggregateRoot(int id)
            : base(id)
        {
        }
    }
}
