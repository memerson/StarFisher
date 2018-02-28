using System;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal class AwardsLuncheonInviteeListExcelFile : ExcelFileBase
    {
        public AwardsLuncheonInviteeListExcelFile(NominationList nominationList)
            : base((com, worksheet) => BuildWorksheet(com,
                nominationList ?? throw new ArgumentNullException(nameof(nominationList)),
                worksheet))
        {
        }

        private static void BuildWorksheet(ComObjectManager com, NominationList nominationList, Worksheet workSheet)
        {
            var cells = com.Get(() => workSheet.Cells);

            var attendees = nominationList
                .AwardsLuncheonInvitees
                .OrderBy(i => i.OfficeLocation.ToString())
                .ThenBy(i => i.Name.FullName)
                .ToList();

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Nominee Name");
            SetCellValue(cells, 1, 2, @"Nominee Office");
            SetCellValue(cells, 1, 3, @"Nominee Email Address");

            for (var i = 0; i < attendees.Count; ++i)
            {
                var rowNumber = i + 2;
                var attendee = attendees[i];

                SetCellValue(cells, rowNumber, 1, attendee.Name.FullName);
                SetCellValue(cells, rowNumber, 2, attendee.OfficeLocation.ToString());
                SetCellValue(cells, rowNumber, 3, attendee.EmailAddress.ToString());
            }
        }
    }
}