using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Perisistence;
using StarFisher.Domain.Utilities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate
{
    public class AwardWinnerListRepository
    {
        private readonly DirectoryPath _workingDirectoryPath;

        public AwardWinnerListRepository(DirectoryPath workingDirectoryPath)
        {
            _workingDirectoryPath = workingDirectoryPath ?? throw new ArgumentNullException(nameof(workingDirectoryPath));
        }

        public void Save(AwardWinnerList awardWinnerList)
        {
            if (awardWinnerList == null)
                throw new ArgumentNullException(nameof(awardWinnerList));

            var directoryPath = GetDirectoryPath(awardWinnerList.Year, awardWinnerList.Quarter);

            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, DateTime.Now.Ticks + ".json");

            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, new AwardWinnerListDto(awardWinnerList));
            }
        }

        public IReadOnlyList<DateTime> ListVersions(Year year, Quarter quarter)
        {
            var versions = new List<DateTime>();
            var directoryPath = GetDirectoryPath(year, quarter);

            if (!Directory.Exists(directoryPath))
                return versions;
            
            foreach (var filePath in Directory.GetFiles(directoryPath))
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);

                if (long.TryParse(fileName, out long version))
                    versions.Add(new DateTime(version));
            }

            return versions;
        }

        public AwardWinnerList Get(Year year, Quarter quarter, DateTime version)
        {
            var directoryPath = GetDirectoryPath(year, quarter);
            var filePath = Path.Combine(directoryPath, version.Ticks + ".json");

            if (!File.Exists(filePath))
                return null;

            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                var awardWinnerListDto = (AwardWinnerListDto)serializer.Deserialize(file, typeof(AwardWinnerListDto));
                return awardWinnerListDto.ToAwardWinnerList();
            }
        }

        public AwardWinnerList GetLatestVersion(Year year, Quarter quarter)
        {
            var versions = ListVersions(year, quarter);
            var latestVersion = versions.SafeMax(v => v);

            return Get(year, quarter, latestVersion);
        }

        private string GetDirectoryPath(Year year, Quarter quarter)
        {
            return Path.Combine(_workingDirectoryPath.Value, year.ToString(), quarter.ToString(), "Winners");
        }
    }
}
