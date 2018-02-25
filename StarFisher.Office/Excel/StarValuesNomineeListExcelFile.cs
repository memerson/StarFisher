using System;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal class StarValuesNomineeListExcelFile : ExcelFileBase
    {
        public StarValuesNomineeListExcelFile(NominationList nominationList)
            : base((com, worksheet) => BuildWorksheet(com,
                nominationList ?? throw new ArgumentNullException(nameof(nominationList)),
                worksheet))
        {
        }

        private static void BuildWorksheet(ComObjectManager com, NominationList nominationList, Worksheet workSheet)
        {
            var cells = com.Get(() => workSheet.Cells);

            var nominees = nominationList
                .GetNomineesForAward(AwardType.StarValues, true)
                .OrderBy(i => i.OfficeLocation.ToString())
                .ThenBy(i => i.Name.FullName)
                .ToList();

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Nominee Name");
            SetCellValue(cells, 1, 2, @"Nominee Office");

            for (var i = 0; i < nominees.Count; ++i)
            {
                var rowNumber = i + 2;
                var nominee = nominees[i];

                SetCellValue(cells, rowNumber, 1, nominee.Name.FullName);
                SetCellValue(cells, rowNumber, 2, nominee.OfficeLocation.ToString());
            }
        }
    }
}