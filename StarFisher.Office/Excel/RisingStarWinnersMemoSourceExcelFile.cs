using System;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal class RisingStarWinnersMemoSourceExcelFile : ExcelFileBase
    {
        public RisingStarWinnersMemoSourceExcelFile(NominationList nominationList)
            : base((com, worksheet) => BuildWorksheet(com,
                nominationList ?? throw new ArgumentNullException(nameof(nominationList)),
                worksheet))
        {
        }

        private static void BuildWorksheet(ComObjectManager com, NominationList nominationList, Worksheet worksheet)
        {
            var cells = com.Get(() => worksheet.Cells);

            var awardWinners = nominationList.RisingStarAwardWinners;

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Nominee Name");
            SetCellValue(cells, 1, 2, @"Nominee Office");

            for (var i = 0; i < awardWinners.Count; ++i)
            {
                var rowNumber = i + 2;
                var awardWinner = awardWinners[i];

                SetCellValue(cells, rowNumber, 1, awardWinner.Name.FullName);
                SetCellValue(cells, rowNumber, 2, awardWinner.OfficeLocation.Name);
            }
        }
    }
}