using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.NominationListAggregate.Entities;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal abstract class NominationNotificationEmailSourceExcelFileBase : ExcelFileBase
    {
        protected NominationNotificationEmailSourceExcelFileBase(AwardsPeriod awardsPeriod,
            IEnumerable<Nomination> nominations)
            : base((com, worksheet) => BuildWorksheet(com,
                awardsPeriod ?? throw new ArgumentNullException(nameof(awardsPeriod)),
                nominations ?? throw new ArgumentNullException(nameof(nominations)),
                worksheet))
        {
        }

        private static void BuildWorksheet(ComObjectManager com, AwardsPeriod awardsPeriod,
            IEnumerable<Nomination> nominations, Worksheet worksheet)
        {
            var cells = com.Get(() => worksheet.Cells);

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Year");
            SetCellValue(cells, 1, 2, @"Quarter");
            SetCellValue(cells, 1, 3, @"Submitted by");
            SetCellValue(cells, 1, 4, @"NOMINEE'S NAME");
            SetCellValue(cells, 1, 5, @"NOMINEE'S OFFICE");
            SetCellValue(cells, 1, 6, @"Continuously_Improving");
            SetCellValue(cells, 1, 7, @"Driving_Innovation");
            SetCellValue(cells, 1, 8, @"Delighting_Customers");
            SetCellValue(cells, 1, 9, @"Behaving_with_Integrity");
            SetCellValue(cells, 1, 10, @"Delivering_Meaningful_Outcomes");
            SetCellValue(cells, 1, 11, @"Streaming_Good");
            SetCellValue(cells, 1, 12, @"WRITE-UP");
            SetCellValue(cells, 1, 13, @"Email");

            var rowNumber = 2;
            foreach (var nomination in nominations)
            {
                SetCellValue(cells, rowNumber, 1, awardsPeriod.Year.ToString());
                SetCellValue(cells, rowNumber, 2, awardsPeriod.Quarter.Abbreviation);
                SetCellValue(cells, rowNumber, 3, nomination.NominatorName.FullName);
                SetCellValue(cells, rowNumber, 4, nomination.NomineeName.FullName);
                SetCellValue(cells, rowNumber, 5, nomination.NomineeOfficeLocation.ToString());
                SetCellValue(cells, rowNumber, 6, GetCompanyValue(nomination, CompanyValue.ContinuouslyImproving));
                SetCellValue(cells, rowNumber, 7, GetCompanyValue(nomination, CompanyValue.DrivingInnovation));
                SetCellValue(cells, rowNumber, 8, GetCompanyValue(nomination, CompanyValue.DelightingCustomers));
                SetCellValue(cells, rowNumber, 9, GetCompanyValue(nomination, CompanyValue.BehavingWithIntegrity));
                SetCellValue(cells, rowNumber, 10, GetCompanyValue(nomination, CompanyValue.DeliveringMeaningfulOutcomes));
                SetCellValue(cells, rowNumber, 11, GetCompanyValue(nomination, CompanyValue.StreamingGood));
                SetCellValue(cells, rowNumber, 12, nomination.WriteUp.ToString());
                SetCellValue(cells, rowNumber, 13, nomination.NomineeEmailAddress.ToString());

                ++rowNumber;
            }
        }
    }
}