using System;
using System.Collections.Generic;
using StarFisher.Domain.QuarterlyAwards;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Context
{
    public interface IAwardWinnerListContext
    {
        bool HasAwardWinnerListLoaded { get; }

        AwardWinnerList AwardWinnerList { get; }

        IReadOnlyList<DateTime> ListSnapshotDateTimes();

        void LoadSnapshot(DateTime snapshotDateTime);

        void LoadLatestSnapshot();

        void SaveSnapshot();
    }

    public class AwardWinnerListContext : QuarterlyAwardsAggregateContext<AwardWinnerList, IRepository<AwardWinnerList>>, IAwardWinnerListContext
    {
        private static AwardWinnerListContext _current;

        private AwardWinnerListContext(IRepository<AwardWinnerList> awardWinnerRepository, Year year, Quarter quarter)
            : base(awardWinnerRepository, year, quarter) { }

        public static void Initialize(IRepository<AwardWinnerList> awardWinnerRepository, Year year, Quarter quarter)
        {
            _current = new AwardWinnerListContext(awardWinnerRepository, year, quarter);
        }

        public static bool IsInitialized => _current != null;

        public static AwardWinnerListContext Current
        {
            get
            {
                if (!IsInitialized)
                    throw new InvalidOperationException("NominationListContext not yet initialized.");

                return _current;
            }
        }

        public bool HasAwardWinnerListLoaded => HasAggregateRootLoaded;

        public AwardWinnerList AwardWinnerList => AggregateRoot;
    }
}
