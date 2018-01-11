using System;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.ValueObjects;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal class VotingGuideSourceExcelFile : BaseExcelFile
    {
        public VotingGuideSourceExcelFile(NominationList nominationList, EmployeeType employeeType)
            : base((com, worksheet) => BuildWorksheet(com,
                nominationList ?? throw new ArgumentNullException(nameof(nominationList)),
                employeeType ?? throw new ArgumentNullException(nameof(employeeType)),
                worksheet))
        { }

        private static void BuildWorksheet(ComObjectManager com, NominationList nominationList, EmployeeType employeeType, Worksheet worksheet)
        {
            var cells = com.Get(() => worksheet.Cells);
            var nominations = nominationList.Nominations
                .Where(n => n.NomineeEmployeeType == employeeType)
                .ToList();

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Record_");
            SetCellValue(cells, 1, 2, @"Nomination_IDs");
            SetCellValue(cells, 1, 3, @"Learning_Culture");
            SetCellValue(cells, 1, 4, @"Innovation");
            SetCellValue(cells, 1, 5, @"Customer_Focus");
            SetCellValue(cells, 1, 6, @"Individual_Integrity");
            SetCellValue(cells, 1, 7, @"Performance");
            SetCellValue(cells, 1, 8, @"WRITEUP");

            for (var i = 0; i < nominations.Count; ++i)
            {
                var rowNumber = i + 2;
                var nomination = nominations[i];

                SetCellValue(cells, rowNumber, 1, nomination.Id);
                SetCellValue(cells, rowNumber, 2, nomination.VotingIdentifier.ToString());
                SetCellValue(cells, rowNumber, 3, GetCompanyValue(nomination, CompanyValue.LearningCulture));
                SetCellValue(cells, rowNumber, 4, GetCompanyValue(nomination, CompanyValue.Innovation));
                SetCellValue(cells, rowNumber, 5, GetCompanyValue(nomination, CompanyValue.CustomerFocus));
                SetCellValue(cells, rowNumber, 6, GetCompanyValue(nomination, CompanyValue.IndividualIntegrity));
                SetCellValue(cells, rowNumber, 7, GetCompanyValue(nomination, CompanyValue.Performance));
                SetCellValue(cells, rowNumber, 8, nomination.WriteUp.ToString());
            }
        }
    }
}
