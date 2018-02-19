﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinqToExcel;
using Newtonsoft.Json;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Persistence;
using StarFisher.Domain.Utilities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate
{
    public interface INominationListRepository
    {
        NominationList LoadSurveyExport(FilePath filePath, Year year, Quarter quarter);
        void SaveSnapshot(NominationList nominationList);
        int GetSnapshotCount(Year year, Quarter quarter);
        IReadOnlyList<SnapshotSummary> ListSnapshotSummaries(Year year, Quarter quarter);
        NominationList GetSnapshot(Year year, Quarter quarter, SnapshotSummary snapshotSummary);
        NominationList GetLatestSnapshot(Year year, Quarter quarter);
    }

    public class NominationListRepository : INominationListRepository
    {
        private readonly DirectoryPath _workingDirectoryPath;

        public NominationListRepository(DirectoryPath workingDirectoryPath)
        {
            _workingDirectoryPath = workingDirectoryPath ??
                                    throw new ArgumentNullException(nameof(workingDirectoryPath));
        }

        public NominationList LoadSurveyExport(FilePath filePath, Year year, Quarter quarter)
        {
            var excel = new ExcelQueryFactory(filePath.Value);
            excel.ReadOnly = true;

            var worksheet = excel.Worksheet(0);
            var nominations = new List<Nomination>(100);
            var rowNumber = 0;

            // It can't handle the OrderBy until we read all the rows into memory.
            var rows = worksheet.Skip(1).ToList().OrderBy(r => r[12].ToString());

            foreach (var row in rows)
            {
                ++rowNumber;
                nominations.Add(LoadNominationFromSurveyExport(row, rowNumber));
            }

            var nominationList = new NominationList(year, quarter, nominations);
            SaveSnapshot(nominationList);
            return nominationList;
        }

        public void SaveSnapshot(NominationList nominationList)
        {
            if (nominationList == null)
                throw new ArgumentNullException(nameof(nominationList));

            if (!nominationList.IsDirty)
                return;

            var directoryPath = GetDirectoryPath(nominationList.Year, nominationList.Quarter);

            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, DateTime.Now.Ticks + ".json");

            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, new NominationListDto(nominationList));
            }

            nominationList.SetCurrent();
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
                var lastChangeSummary = dto.LastChangeSummary;
                snapshotSummaries.Add(new SnapshotSummary(snapshotDateTime, lastChangeSummary));
            }

            return snapshotSummaries;
        }

        public NominationList GetSnapshot(Year year, Quarter quarter, SnapshotSummary snapshotSummary)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));
            if (snapshotSummary == null)
                throw new ArgumentNullException(nameof(snapshotSummary));

            return GetSnapshotDto(year, quarter, snapshotSummary.DateTime).ToNominationList();
        }

        public NominationList GetLatestSnapshot(Year year, Quarter quarter)
        {
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));

            var snapshotDateTimes = ListSnapshotSummaries(year, quarter);
            var latestSnapshotDateTime = snapshotDateTimes.SafeMax(v => v);

            return GetSnapshot(year, quarter, latestSnapshotDateTime);
        }

        private static Nomination LoadNominationFromSurveyExport(Row row, int rowNumber)
        {
            var isAnonymousNominator = row[10] != @"Display My Name (Recommended)";
            var nominatorName = PersonName.CreateForNominator(row[9], isAnonymousNominator);
            var nomineeName = PersonName.Create(row[12]);
            var awardType = AwardType.Create(row[13]);
            var nomineeOfficeLocation = OfficeLocation.Create(row[15]);
            var hasLearningCulture = !string.IsNullOrWhiteSpace(row[16]);
            var hasInnovation = !string.IsNullOrWhiteSpace(row[17]);
            var hasCustomerFocus = !string.IsNullOrWhiteSpace(row[18]);
            var hasIndividualIntegrity = !string.IsNullOrWhiteSpace(row[19]);
            var hasPerformance = !string.IsNullOrWhiteSpace(row[20]);
            var writeUp = NominationWriteUp.Create(nomineeName, row[21]);
            var writeUpSummary = NominationWriteUpSummary.Create(row[23]);

            var companyValues = GetCompanyValues(hasLearningCulture, hasInnovation, hasCustomerFocus,
                hasIndividualIntegrity, hasPerformance);

            var nominee = Person.Create(nomineeName, nomineeOfficeLocation, nomineeName.DerivedEmailAddress);

            var nomination = new Nomination(rowNumber, NomineeVotingIdentifier.Unknown, nominee, awardType,
                nominatorName, companyValues, writeUp, writeUpSummary);

            return nomination;
        }

        private static IEnumerable<CompanyValue> GetCompanyValues(bool hasLearningCulture, bool hasInnovation,
            bool hasCustomerFocus,
            bool hasIndividualIntegrity, bool hasPerformance)
        {
            var companyValues = new List<CompanyValue>(5);
            if (hasLearningCulture)
                companyValues.Add(CompanyValue.LearningCulture);
            if (hasInnovation)
                companyValues.Add(CompanyValue.Innovation);
            if (hasCustomerFocus)
                companyValues.Add(CompanyValue.CustomerFocus);
            if (hasIndividualIntegrity)
                companyValues.Add(CompanyValue.IndividualIntegrity);
            if (hasPerformance)
                companyValues.Add(CompanyValue.Performance);
            return companyValues;
        }

        private string GetDirectoryPath(Year year, Quarter quarter)
        {
            return Path.Combine(_workingDirectoryPath.Value, year.ToString(), quarter.ToString(), @"Snapshots");
        }

        private NominationListDto GetSnapshotDto(Year year, Quarter quarter, DateTime snapshotDateTime)
        {
            var directoryPath = GetDirectoryPath(year, quarter);
            var filePath = Path.Combine(directoryPath, snapshotDateTime.Ticks + ".json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException(@"Snapshot file not found.", filePath);

            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                return (NominationListDto) serializer.Deserialize(file, typeof(NominationListDto));
            }
        }
    }
}