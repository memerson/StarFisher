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

    public class NominationListContext : INominationListContext
    {
        private readonly Quarter _quarter;
        private readonly INominationListRepository _repository;
        private readonly Year _year;

        private NominationList _nominationList;

        public NominationListContext(INominationListRepository nominationListRepository, Year year, Quarter quarter)
        {
            _repository = nominationListRepository ?? throw new ArgumentNullException(nameof(nominationListRepository));
            _year = year ?? throw new ArgumentNullException(nameof(year));
            _quarter = quarter ?? throw new ArgumentNullException(nameof(quarter));
        }

        public bool HasNominationListLoaded => _nominationList != null;

        public NominationList NominationList
        {
            get
            {
                if (!HasNominationListLoaded)
                    throw new InvalidOperationException("No NominationList loaded.");

                return _nominationList;
            }

            set => _nominationList = value;
        }

        public void LoadSurveyExport(FilePath filePath)
        {
            NominationList = _repository.LoadSurveyExport(filePath, _year, _quarter);
        }

        public int GetSnapshotCount()
        {
            return _repository.GetSnapshotCount(_year, _quarter);
        }

        public IReadOnlyList<SnapshotSummary> ListSnapshotSummaries()
        {
            return _repository.ListSnapshotSummaries(_year, _quarter);
        }

        public void LoadSnapshot(SnapshotSummary snapshotSummary)
        {
            NominationList = _repository.GetSnapshot(_year, _quarter, snapshotSummary);
        }

        public void LoadLatestSnapshot()
        {
            NominationList = _repository.GetLatestSnapshot(_year, _quarter);
        }

        public void SaveSnapshot()
        {
            _repository.SaveSnapshot(NominationList);
        }
    }
}