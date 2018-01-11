using System;

namespace StarFisher.Domain.Common
{
    public abstract class Repository<T> : IDisposable
        where T : AggregateRoot
    {
        public abstract void Dispose();
    }
}
