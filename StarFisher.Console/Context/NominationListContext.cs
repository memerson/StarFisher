using System;
using System.Collections.Generic;
using StarFisher.Domain.QuarterlyAwards;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Context
{
    public interface INominationListContext
    {
        bool HasNominationListLoaded { get; }

        NominationList NominationList { get; }

        void LoadSurveyExport(FilePath filePath);

        int GetSnapshotCount();

        IReadOnlyList<SnapshotSummary> ListSnapshotSummaries();

        void LoadSnapshot(SnapshotSummary snapshotSummary);

        void LoadLatestSnapshot();

        void SaveSnapshot();
    }

    public class NominationListContext : QuarterlyAwardsAggregateContext<NominationList, INominationListRepository>, INominationListContext
    {
        private static NominationListContext _current;

        private NominationListContext(INominationListRepository nominationListRepository, Year year, Quarter quarter)
            : base(nominationListRepository, year, quarter) { }

        public static void Initialize(INominationListRepository nominationListRepository, Year year, Quarter quarter)
        {
            _current = new NominationListContext(nominationListRepository, year, quarter);
        }

        public static bool IsInitialized => _current != null;

        public static NominationListContext Current
        {
            get
            {
                if (!IsInitialized)
                    throw new InvalidOperationException(@"NominationListContext not yet initialized");

                return _current;
            }
        }

        public bool HasNominationListLoaded => HasAggregateRootLoaded;

        public NominationList NominationList => AggregateRoot;

        public void LoadSurveyExport(FilePath filePath)
        {
            AggregateRoot = Repository.LoadSurveyExport(filePath, Year, Quarter);
        }
    }
}
