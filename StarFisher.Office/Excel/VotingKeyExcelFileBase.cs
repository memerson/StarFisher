using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal abstract class VotingKeyExcelFileBase : ExcelFileBase
    {
        protected VotingKeyExcelFileBase(IEnumerable<Nomination> nominations)
            : base((com, worksheet) => BuildWorksheet(com,
                nominations ?? throw new ArgumentNullException(nameof(nominations)),
                worksheet))
        { }

        private static void BuildWorksheet(ComObjectManager com, IEnumerable<Nomination> nominations, Worksheet workSheet)
        {
            var cells = com.Get(() => workSheet.Cells);

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Nominiation ID(s)");
            SetCellValue(cells, 1, 2, @"Nominee Name");
            SetCellValue(cells, 1, 3, @"Nominee Office");

            int rowNumber = 2;
            foreach(var nomination in nominations)
            {
                SetCellValue(cells, rowNumber, 1, nomination.VotingIdentifier.ToString());
                SetCellValue(cells, rowNumber, 2, nomination.NomineeName.FullName);
                SetCellValue(cells, rowNumber, 3, nomination.NomineeOfficeLocation.ToString());

                ++rowNumber;
            }
        }
    }
}
