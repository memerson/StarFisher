using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.ValueObjects;

namespace StarFisher.Office.Excel
{
    public interface IExcelFileFactory
    {
        IExcelFile GetStarValuesVotingGuideSourceExcelFile(NominationList nominationList);

        IExcelFile GetStarValuesVotingKeyExcelFile(NominationList nominationList);
    }

    public class ExcelFileFactory : IExcelFileFactory
    {
        public IExcelFile GetStarValuesVotingGuideSourceExcelFile(NominationList nominationList)
        {
            return new VotingGuideSourceExcelFile(nominationList, EmployeeType.Employee);
        }

        public IExcelFile GetStarValuesVotingKeyExcelFile(NominationList nominationList)
        {
            return new VotingKeyExcelFile(nominationList, EmployeeType.Employee);
        }
    }
}
