using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal class NomineeListExcelFileBase : ExcelFileBase
    {
        public NomineeListExcelFileBase(IEnumerable<Person> nominees)
            : base((com, worksheet) => BuildWorksheet(com,
                nominees ?? throw new ArgumentNullException(nameof(nominees)),
                worksheet))
        {
        }

        private static void BuildWorksheet(ComObjectManager com, IEnumerable<Person> nomineesx, Worksheet workSheet)
        {
            var cells = com.Get(() => workSheet.Cells);

            var orderedNominees = nomineesx
                .OrderBy(i => i.OfficeLocation.ToString())
                .ThenBy(i => i.Name.FullName);

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Nominee Name");
            SetCellValue(cells, 1, 2, @"Nominee Office");

            var rowNumber = 1;

            foreach(var nominee in orderedNominees)
            {
                ++rowNumber;

                SetCellValue(cells, rowNumber, 1, nominee.Name.FullName);
                SetCellValue(cells, rowNumber, 2, nominee.OfficeLocation.ToString());
            }
        }
    }
}