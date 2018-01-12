using System;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal class AwardsLunchInviteeListExcelFile : BaseExcelFile
    {
        public AwardsLunchInviteeListExcelFile(NominationList nominationList, AwardWinnerList awardWinnerList)
            : base((com, worksheet) => BuildWorksheet(com,
                nominationList ?? throw new ArgumentNullException(nameof(nominationList)),
                awardWinnerList ?? throw new ArgumentNullException(nameof(awardWinnerList)),
                worksheet))
        { }

        private static void BuildWorksheet(ComObjectManager com, NominationList nominationList, AwardWinnerList awardWinnerList, Worksheet workSheet)
        {
            var cells = com.Get(() => workSheet.Cells);

            var attendees = nominationList.AwardsLuncheonInvitees
                .Select(n => new
                {
                    Name = n.NomineeName,
                    OfficeLocation = n.NomineeOfficeLocation,
                    EmailAddress = n.NomineeEmailAddress
                })
                .Union(awardWinnerList.PerformanceAwardWinners
                    .Select(w => new { w.Name, w.OfficeLocation, w.EmailAddress }))
                .OrderBy(i => new { i.OfficeLocation, i.Name })
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
                SetCellValue(cells, rowNumber, 2, attendee.OfficeLocation);
                SetCellValue(cells, rowNumber, 3, attendee.EmailAddress.ToString());
            }
        }
    }
}
