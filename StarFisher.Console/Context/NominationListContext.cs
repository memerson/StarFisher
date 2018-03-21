using System;
using System.Collections.Generic;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

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
        private readonly AwardCategory _awardCategory;
        private readonly INominationListRepository _repository;

        private NominationList _nominationList;

        public NominationListContext(INominationListRepository nominationListRepository, AwardCategory awardCategory)
        {
            _repository = nominationListRepository ?? throw new ArgumentNullException(nameof(nominationListRepository));
            _awardCategory = awardCategory ?? throw new ArgumentNullException(nameof(awardCategory));
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
            NominationList = _repository.LoadSurveyExport(_awardCategory, filePath);
        }

        public int GetSnapshotCount()
        {
            return _repository.GetSnapshotCount();
        }

        public IReadOnlyList<SnapshotSummary> ListSnapshotSummaries()
        {
            return _repository.ListSnapshotSummaries();
        }

        public void LoadSnapshot(SnapshotSummary snapshotSummary)
        {
            NominationList = _repository.GetSnapshot(snapshotSummary);
        }

        public void LoadLatestSnapshot()
        {
            NominationList = _repository.GetLatestSnapshot();
        }

        public void SaveSnapshot()
        {
            _repository.SaveSnapshot(NominationList);
        }
    }
}