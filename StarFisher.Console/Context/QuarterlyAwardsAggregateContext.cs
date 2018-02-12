using System;
using System.Collections.Generic;
using StarFisher.Domain.Common;
using StarFisher.Domain.QuarterlyAwards;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Context
{
    public abstract class QuarterlyAwardsAggregateContext<TAggregateRoot, TRepo>
        where TRepo : class, IRepository<TAggregateRoot>
        where TAggregateRoot : AggregateRoot
    {
        private TAggregateRoot _aggregateRoot;

        protected QuarterlyAwardsAggregateContext(TRepo repo, Year year, Quarter quarter)
        {
            Repository = repo ?? throw new ArgumentNullException(nameof(repo));
            Year = year ?? throw new ArgumentNullException(nameof(year));
            Quarter = quarter ?? throw new ArgumentNullException(nameof(quarter));
        }

        public int GetSnapshotCount()
        {
            return Repository.GetSnapshotCount(Year, Quarter);
        }

        public IReadOnlyList<SnapshotSummary> ListSnapshotSummaries()
        {
            return Repository.ListSnapshotSummaries(Year, Quarter);
        }

        public void LoadSnapshot(SnapshotSummary snapshotSummary)
        {
            AggregateRoot = Repository.GetSnapshot(Year, Quarter, snapshotSummary);
        }

        public void LoadLatestSnapshot()
        {
            AggregateRoot = Repository.GetLatestSnapshot(Year, Quarter);
        }

        public void SaveSnapshot()
        {
            Repository.SaveSnapshot(AggregateRoot);
        }

        protected bool HasAggregateRootLoaded => _aggregateRoot != null;

        protected TAggregateRoot AggregateRoot
        {
            get
            {
                if (!HasAggregateRootLoaded)
                    throw new InvalidOperationException("No NominationList loaded.");

                return _aggregateRoot;
            }

            set => _aggregateRoot = value;
        }

        protected TRepo Repository { get; }

        protected Year Year { get; }

        protected Quarter Quarter { get; }
    }
}
