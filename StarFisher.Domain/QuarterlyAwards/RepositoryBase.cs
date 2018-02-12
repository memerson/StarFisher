using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StarFisher.Domain.Common;
using StarFisher.Domain.Utilities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards
{
    public interface IRepository<T> 
        where T : AggregateRoot
    {
        void SaveSnapshot(T aggregateRoot);

        int GetSnapshotCount(Year year, Quarter quarter);

        IReadOnlyList<SnapshotSummary> ListSnapshotSummaries(Year year, Quarter quarter);

        T GetSnapshot(Year year, Quarter quarter, SnapshotSummary snapshotSummary);

        T GetLatestSnapshot(Year year, Quarter quarter);
    }

    public abstract class RepositoryBase<T> : IRepository<T> 
        where T : AggregateRoot
    {
        private readonly Type _dtoType;
        private readonly DirectoryPath _workingDirectoryPath;
        private readonly string _aggregateDirectoryName;

        protected RepositoryBase(Type dtoType, DirectoryPath workingDirectoryPath, string aggregateDirectoryName)
        {
            if (string.IsNullOrWhiteSpace(aggregateDirectoryName))
                throw new ArgumentException(aggregateDirectoryName);

            _dtoType = dtoType ?? throw new ArgumentNullException(nameof(dtoType));
            _workingDirectoryPath = workingDirectoryPath ?? throw new ArgumentNullException(nameof(workingDirectoryPath));
            _aggregateDirectoryName = aggregateDirectoryName;
        }

        public void SaveSnapshot(T aggregateRoot)
        {
            if (aggregateRoot == null)
                throw new ArgumentNullException(nameof(aggregateRoot));

            if (!aggregateRoot.IsDirty)
                return;

            var directoryPath = GetDirectoryPath(GetYear(aggregateRoot), GetQuarter(aggregateRoot));

            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, DateTime.Now.Ticks + ".json");

            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, GetDtoFromAggregateRoot(aggregateRoot));
            }

            aggregateRoot.SetCurrent();
        }

        public int GetSnapshotCount(Year year, Quarter quarter)
        {
            var directoryPath = GetDirectoryPath(year, quarter);

            if (!Directory.Exists(directoryPath))
                return 0;

            return Directory.GetFiles(directoryPath)
                .Select(Path.GetFileNameWithoutExtension)
                .Count(fn => long.TryParse(fn, out long unused));
        }

        public IReadOnlyList<SnapshotSummary> ListSnapshotSummaries(Year year, Quarter quarter)
        {
            var snapshotSummaries = new List<SnapshotSummary>();
            var directoryPath = GetDirectoryPath(year, quarter);

            if (!Directory.Exists(directoryPath))
                return snapshotSummaries;

            foreach (var filePath in Directory.GetFiles(directoryPath))
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);

                if (!long.TryParse(fileName, out long snapshotTicks))
                    continue;

                var snapshotDateTime = new DateTime(snapshotTicks);
                var dto = GetSnapshotDto(year, quarter, snapshotDateTime);
                var lastChangeSummary = GetLastChangeSummaryFromDto(dto);
                snapshotSummaries.Add(new SnapshotSummary(snapshotDateTime, lastChangeSummary));
            }

            return snapshotSummaries;
        }

        public T GetSnapshot(Year year, Quarter quarter, SnapshotSummary snapshotSummary)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));
            if (snapshotSummary == null)
                throw new ArgumentNullException(nameof(snapshotSummary));

            return GetAggregateRootFromDto(GetSnapshotDto(year, quarter, snapshotSummary.DateTime));
        }

        public T GetLatestSnapshot(Year year, Quarter quarter)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));

            var snapshotDateTimes = ListSnapshotSummaries(year, quarter);
            var latestSnapshotDateTime = snapshotDateTimes.SafeMax(v => v);

            return GetSnapshot(year, quarter, latestSnapshotDateTime);
        }

        private string GetDirectoryPath(Year year, Quarter quarter)
        {
            return Path.Combine(_workingDirectoryPath.Value, year.ToString(), quarter.ToString(), _aggregateDirectoryName);
        }

        private object GetSnapshotDto(Year year, Quarter quarter, DateTime snapshotDateTime)
        {
            var directoryPath = GetDirectoryPath(year, quarter);
            var filePath = Path.Combine(directoryPath, snapshotDateTime.Ticks + ".json");

            if (!File.Exists(filePath))
                return null;

            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                var dto = serializer.Deserialize(file, _dtoType);
                return dto;
            }
        }

        protected abstract T GetAggregateRootFromDto(object dto);

        protected abstract object GetDtoFromAggregateRoot(T aggregateRoot);

        protected abstract Quarter GetQuarter(T aggregateRoot);

        protected abstract Year GetYear(T aggregateRoot);

        protected abstract string GetLastChangeSummaryFromDto(object dto);
    }
}
