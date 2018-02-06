using System.Collections.Generic;
using System.Linq;
using LinqToExcel;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Persistence;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.NominationListAggregate
{
    public interface INominationListRepository : IRepository<NominationList>
    {
        NominationList LoadSurveyExport(FilePath filePath, Year year, Quarter quarter);
    }

    public class NominationListRepository : RepositoryBase<NominationList>, INominationListRepository
    {
        public NominationListRepository(DirectoryPath workingDirectoryPath)
            : base(typeof(NominationListDto), workingDirectoryPath, "Nominations") { }

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

        protected override NominationList GetAggregateRootFromDto(object dto)
        {
            var nominationListDto = dto as NominationListDto;
            return nominationListDto?.ToNominationList();
        }

        protected override object GetDtoFromAggregateRoot(NominationList nominationList)
        {
            return new NominationListDto(nominationList);
        }

        protected override Quarter GetQuarter(NominationList nominationList)
        {
            return nominationList.Quarter;
        }

        protected override Year GetYear(NominationList nominationList)
        {
            return nominationList.Year;
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

            var companyValues = GetCompanyValues(hasLearningCulture, hasInnovation, hasCustomerFocus, hasIndividualIntegrity, hasPerformance);

            var nominee = Person.Create(nomineeName, nomineeOfficeLocation, nomineeName.DerivedEmailAddress);

            var nomination = new Nomination(rowNumber, NomineeVotingIdentifier.Unknown, nominee, awardType,
                nominatorName, companyValues, writeUp, writeUpSummary);

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
