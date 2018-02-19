using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.ValueObjects;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal abstract class CertificatesSourceExcelFileBase : ExcelFileBase
    {
        protected CertificatesSourceExcelFileBase(Year year, Quarter quarter, IEnumerable<AwardWinner> awardWinners)
            : base((com, worksheet) => BuildWorksheet(com,
                year ?? throw new ArgumentNullException(nameof(year)),
                quarter ?? throw new ArgumentNullException(nameof(quarter)),
                awardWinners ?? throw new ArgumentNullException(nameof(awardWinners)),
                worksheet))
        {
        }

        private static void BuildWorksheet(ComObjectManager com, Year year, Quarter quarter,
            IEnumerable<AwardWinner> awardWinners, Worksheet worksheet)
        {
            var awardWinnersToInclude = awardWinners
                .Where(w => OfficeLocation.OfficeLocationsForCertificatePrinting.Any(ol => ol == w.OfficeLocation));

            var cells = com.Get(() => worksheet.Cells);

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Year");
            SetCellValue(cells, 1, 2, @"Quarter");
            SetCellValue(cells, 1, 3, @"Name");

            var rowNumber = 2;
            foreach (var awardWinner in awardWinnersToInclude)
            {
                SetCellValue(cells, rowNumber, 1, year.ToString());
                SetCellValue(cells, rowNumber, 2, quarter.FullName);
                SetCellValue(cells, rowNumber, 3, awardWinner.Name.FullName);

                ++rowNumber;
            }
        }
    }
}