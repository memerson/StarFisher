using System;
using System.Collections.Generic;
using System.IO;
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

        IReadOnlyList<DateTime> ListSnapshotDateTimes(Year year, Quarter quarter);

        T GetSnapshot(Year year, Quarter quarter, DateTime snapshotDateTime);

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

        public IReadOnlyList<DateTime> ListSnapshotDateTimes(Year year, Quarter quarter)
        {
            var snapshotDateTimes = new List<DateTime>();
            var directoryPath = GetDirectoryPath(year, quarter);

            if (!Directory.Exists(directoryPath))
                return snapshotDateTimes;

            foreach (var filePath in Directory.GetFiles(directoryPath))
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);

                if (long.TryParse(fileName, out long snapshotDateTime))
                    snapshotDateTimes.Add(new DateTime(snapshotDateTime));
            }

            return snapshotDateTimes;
        }

        public T GetSnapshot(Year year, Quarter quarter, DateTime snapshotDateTime)
        {
            var directoryPath = GetDirectoryPath(year, quarter);
            var filePath = Path.Combine(directoryPath, snapshotDateTime.Ticks + ".json");

            if (!File.Exists(filePath))
                return null;

            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                var dto = serializer.Deserialize(file, _dtoType);
                return GetAggregateRootFromDto(dto);
            }
        }

        public T GetLatestSnapshot(Year year, Quarter quarter)
        {
            var snapshotDateTimes = ListSnapshotDateTimes(year, quarter);
            var latestSnapshotDateTime = snapshotDateTimes.SafeMax(v => v);

            return GetSnapshot(year, quarter, latestSnapshotDateTime);
        }

        private string GetDirectoryPath(Year year, Quarter quarter)
        {
            return Path.Combine(_workingDirectoryPath.Value, year.ToString(), quarter.ToString(), _aggregateDirectoryName);
        }

        protected abstract T GetAggregateRootFromDto(object dto);

        protected abstract object GetDtoFromAggregateRoot(T aggregateRoot);

        protected abstract Quarter GetQuarter(T aggregateRoot);

        protected abstract Year GetYear(T aggregateRoot);
    }
}
