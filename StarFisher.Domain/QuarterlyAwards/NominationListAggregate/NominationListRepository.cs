using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinqToExcel;
using Newtonsoft.Json;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Persistence;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.ValueObjects;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate
{
    public interface INominationListRepository
    {
        NominationList LoadSurveyExport(FilePath filePath, Quarter quarter, Year year);

        void SaveProgress(NominationList nominationList);

        NominationList RecoverProgress(Quarter quarter, Year year);
    }

    public class NominationListRepository : INominationListRepository
    {
        private readonly DirectoryInfo _saveDirectory;

        public NominationListRepository()
        {
            var saveDirectoryPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"Saves");
            _saveDirectory = Directory.CreateDirectory(saveDirectoryPath);
        }

        public NominationList LoadSurveyExport(FilePath filePath, Quarter quarter, Year year)
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

            var nominationList = new NominationList(quarter, year, nominations);
            SaveProgress(nominationList);
            return nominationList;
        }

        public void SaveProgress(NominationList nominationList)
        {
            var filePath = Path.Combine(_saveDirectory.FullName, nominationList.Year.ToString(),
                nominationList.Quarter.ToString(),
                DateTime.Now.Ticks + ".json");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, new NominationListDto(nominationList));
            }
        }

        public NominationList RecoverProgress(Quarter quarter, Year year)
        {
            var directoryPath = Path.Combine(_saveDirectory.FullName, year.ToString(),
                quarter.ToString());

            var directoryInfo = new DirectoryInfo(directoryPath);

            if (!directoryInfo.Exists)
                return null;

            var files = directoryInfo.GetFiles();

            if (files.Length == 0)
                return null;

            var mostRecentSaveFileInfo = files.OrderByDescending(f => f.FullName).First();

            using (var file = mostRecentSaveFileInfo.OpenText())
            {
                var serializer = new JsonSerializer();
                var nominationListDto = (NominationListDto) serializer.Deserialize(file, typeof(NominationListDto));
                return nominationListDto.ToNominationList();
            }
        }

        private static Nomination LoadNominationFromSurveyExport(Row row, int rowNumber)
        {
            var isAnonymousNominator = row[10] != @"Display My Name (Recommended)";
            var nominatorName = PersonName.CreateForNominator(row[9], isAnonymousNominator);
            var nomineeName = PersonName.Create(row[12]);
            var nomineeEmployeeType = EmployeeType.Create(row[13]);
            var nomineeOfficeLocation = OfficeLocation.Create(row[15]);
            var hasLearningCulture = !string.IsNullOrWhiteSpace(row[16]);
            var hasInnovation = !string.IsNullOrWhiteSpace(row[17]);
            var hasCustomerFocus = !string.IsNullOrWhiteSpace(row[18]);
            var hasIndividualIntegrity = !string.IsNullOrWhiteSpace(row[19]);
            var hasPerformance = !string.IsNullOrWhiteSpace(row[20]);
            var writeUp = NominationWriteUp.Create(nomineeName, row[21]);
            var writeUpSummary = NominationWriteUpSummary.Create(row[23]);

            var companyValues = GetCompanyValues(hasLearningCulture, hasInnovation, hasCustomerFocus, hasIndividualIntegrity, hasPerformance);

            var nomination = new Nomination(rowNumber, NomineeVotingIdentifier.Unknown, nomineeName, nominatorName,
                nomineeEmployeeType, nomineeOfficeLocation,
                companyValues, writeUp, writeUpSummary,
                EmailAddress.Create(nomineeName.FirstName, nomineeName.LastName));

            return nomination;
        }

        private static IEnumerable<CompanyValue> GetCompanyValues(bool hasLearningCulture, bool hasInnovation, bool hasCustomerFocus,
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
    }
}
