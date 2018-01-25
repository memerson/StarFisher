using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal abstract class CertificateSourceExcelFileBase : ExcelFileBase
    {
        protected CertificateSourceExcelFileBase(Quarter quarter, IEnumerable<AwardWinnerBase> awardWinners, IEnumerable<OfficeLocation> officeLocations)
            : base((com, worksheet) => BuildWorksheet(com,
                quarter ?? throw new ArgumentNullException(nameof(quarter)),
                awardWinners ?? throw new ArgumentNullException(nameof(awardWinners)),
                officeLocations ?? throw new ArgumentNullException(nameof(officeLocations)),
                worksheet))
        { }

        private static void BuildWorksheet(ComObjectManager com, Quarter quarter, IEnumerable<AwardWinnerBase> awardWinners, IEnumerable<OfficeLocation> officeLocations, Worksheet worksheet)
        {
            var cells = com.Get(() => worksheet.Cells);

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Quarter");
            SetCellValue(cells, 1, 2, @"NOMINEE'S NAME");
            SetCellValue(cells, 1, 3, @"NOMINEE'S OFFICE");

            var rowNumber = 2;
            foreach (var awardWinner in awardWinners.Where(w => officeLocations.Any(ol => ol == w.OfficeLocation)))
            {
                SetCellValue(cells, rowNumber, 1, quarter.FullName);
                SetCellValue(cells, rowNumber, 2, awardWinner.Name.FullName);
                SetCellValue(cells, rowNumber, 3, awardWinner.OfficeLocation.ToString());

                ++rowNumber;
            }
        }
    }
}
