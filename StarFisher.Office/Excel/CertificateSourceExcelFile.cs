using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal abstract class CertificateSourceExcelFile : BaseExcelFile
    {
        protected CertificateSourceExcelFile(Quarter quarter, IEnumerable<AwardWinner> awardWinners)
            : base((com, worksheet) => BuildWorksheet(com,
                quarter ?? throw new ArgumentNullException(nameof(quarter)),
                awardWinners ?? throw new ArgumentNullException(nameof(awardWinners)),
                worksheet))
        { }

        private static void BuildWorksheet(ComObjectManager com, Quarter quarter, IEnumerable<AwardWinner> awardWinners, Worksheet worksheet)
        {
            var cells = com.Get(() => worksheet.Cells);

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Quarter");
            SetCellValue(cells, 1, 2, @"NOMINEE'S NAME");
            SetCellValue(cells, 1, 3, @"NOMINEE'S OFFICE");

            var rowNumber = 2;
            foreach (var awardWinner in awardWinners)
            {
                SetCellValue(cells, rowNumber, 1, quarter.FullName);
                SetCellValue(cells, rowNumber, 2, awardWinner.Name.FullName);
                SetCellValue(cells, rowNumber, 3, awardWinner.OfficeLocation.ToString());

                ++rowNumber;
            }
        }
    }
}
