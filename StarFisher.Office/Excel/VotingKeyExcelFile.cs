using System;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal class VotingKeyExcelFile : BaseExcelFile
    {
        public VotingKeyExcelFile(NominationList nominationList, EmployeeType employeeType)
            : base((com, worksheet) => BuildWorksheet(com,
                nominationList ?? throw new ArgumentNullException(nameof(nominationList)),
                employeeType ?? throw new ArgumentNullException(nameof(employeeType)),
                worksheet))
        { }

        private static void BuildWorksheet(ComObjectManager com, NominationList nominationList, EmployeeType employeeType, Worksheet workSheet)
        {
            var cells = com.Get(() => workSheet.Cells);
            var nominations = nominationList.Nominations
                .Where(n => n.NomineeEmployeeType == employeeType)
                .ToList();

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Nominiation ID(s)");
            SetCellValue(cells, 1, 2, @"Nominee Name");
            SetCellValue(cells, 1, 3, @"Nominee Office");

            for (var i = 0; i < nominations.Count; ++i)
            {
                var rowNumber = i + 2;
                var nomination = nominations[i];

                SetCellValue(cells, rowNumber, 1, nomination.VotingIdentifier.ToString());
                SetCellValue(cells, rowNumber, 2, nomination.NomineeName.FullName);
                SetCellValue(cells, rowNumber, 3, nomination.NomineeOfficeLocation.ToString());
            }
        }
    }
}
