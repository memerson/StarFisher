using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal class StarValuesWinnersMemoSourceExcelFile : ExcelFileBase
    {
        public StarValuesWinnersMemoSourceExcelFile(NominationList nominationList)
            : base((com, worksheet) => BuildWorksheet(com,
                nominationList?.Quarter ?? throw new ArgumentNullException(nameof(nominationList)),
                nominationList?.StarValuesAwardWinners ?? throw new ArgumentNullException(nameof(nominationList)),
                worksheet))
        { }

        private static void BuildWorksheet(ComObjectManager com, Quarter quarter,
            IEnumerable<StarValuesAwardWinner> awardWinners, Worksheet worksheet)
        {
            var cells = com.Get(() => worksheet.Cells);

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Quarter");
            SetCellValue(cells, 1, 2, @"Name");
            SetCellValue(cells, 1, 3, @"Office");
            SetCellValue(cells, 1, 4, @"Values");
            SetCellValue(cells, 1, 5, @"WriteUps");

            var rowNumber = 2;
            foreach (var awardWinner in awardWinners)
            {
                var values = string.Join("; ", awardWinner.CompanyValues.Select(cv => cv.ToString()));
                var writeUps = CompileWriteUps(awardWinner.NominationWriteUps);

                SetCellValue(cells, rowNumber, 1, quarter.Abbreviation);
                SetCellValue(cells, rowNumber, 2, awardWinner.Name.FullName);
                SetCellValue(cells, rowNumber, 3, awardWinner.OfficeLocation.ConciseName);
                SetCellValue(cells, rowNumber, 4, values);
                SetCellValue(cells, rowNumber, 5, writeUps);

                ++rowNumber;
            }
        }

        private static string CompileWriteUps(IReadOnlyCollection<NominationWriteUp> writeUps)
        {
            if (writeUps.Count == 1)
                return "Write-Up: " + writeUps.First().Value;

            var count = 1;
            var stringBuilder = new StringBuilder();

            foreach (var writeUp in writeUps)
            {
                if (count > 1)
                    stringBuilder.Append(Environment.NewLine);

                stringBuilder.Append($@"Write-Up {count}:");
                stringBuilder.Append(writeUp.Value);
                ++count;
            }

            return stringBuilder.ToString();
        }
    }
}
