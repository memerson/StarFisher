using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal abstract class CertificatesSourceExcelFileBase : ExcelFileBase
    {
        protected CertificatesSourceExcelFileBase(AwardsPeriod awardsPeriod, IEnumerable<AwardWinner> awardWinners)
            : base((com, worksheet) => BuildWorksheet(com,
                awardsPeriod ?? throw new ArgumentNullException(nameof(awardsPeriod)),
                awardWinners ?? throw new ArgumentNullException(nameof(awardWinners)),
                worksheet))
        {
        }

        private static void BuildWorksheet(ComObjectManager com, AwardsPeriod awardsPeriod,
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
                SetCellValue(cells, rowNumber, 1, awardsPeriod.Year.ToString());
                SetCellValue(cells, rowNumber, 2, awardsPeriod.Quarter.FullName);
                SetCellValue(cells, rowNumber, 3, awardWinner.Name.FullName);

                ++rowNumber;
            }
        }
    }
}