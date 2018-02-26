using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal abstract class NominationNotificationEmailSourceExcelFileBase : ExcelFileBase
    {
        protected NominationNotificationEmailSourceExcelFileBase(Year year, Quarter quarter, IEnumerable<Nomination> nominations)
            : base((com, worksheet) => BuildWorksheet(com,
                year ?? throw new ArgumentNullException(nameof(year)),
                quarter ?? throw new ArgumentNullException(nameof(quarter)),
                nominations ?? throw new ArgumentNullException(nameof(nominations)),
                worksheet))
        {
        }

        private static void BuildWorksheet(ComObjectManager com, Year year, Quarter quarter,
            IEnumerable<Nomination> nominations, Worksheet worksheet)
        {
            var cells = com.Get(() => worksheet.Cells);

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Year");
            SetCellValue(cells, 1, 2, @"Quarter");
            SetCellValue(cells, 1, 3, @"Submitted by");
            SetCellValue(cells, 1, 4, @"NOMINEE'S NAME");
            SetCellValue(cells, 1, 5, @"NOMINEE'S OFFICE");
            SetCellValue(cells, 1, 6, @"Learning Culture");
            SetCellValue(cells, 1, 7, @"Innovation");
            SetCellValue(cells, 1, 8, @"Customer Focus");
            SetCellValue(cells, 1, 9, @"Individual Integrity");
            SetCellValue(cells, 1, 10, @"Performance");
            SetCellValue(cells, 1, 11, @"WRITE-UP");
            SetCellValue(cells, 1, 12, @"Email");

            var rowNumber = 2;
            foreach (var nomination in nominations)
            {
                SetCellValue(cells, rowNumber, 1, year);
                SetCellValue(cells, rowNumber, 2, quarter.Abbreviation);
                SetCellValue(cells, rowNumber, 3, nomination.NominatorName.FullName);
                SetCellValue(cells, rowNumber, 4, nomination.NomineeName.FullName);
                SetCellValue(cells, rowNumber, 5, nomination.NomineeOfficeLocation.ToString());
                SetCellValue(cells, rowNumber, 6, GetCompanyValue(nomination, CompanyValue.LearningCulture));
                SetCellValue(cells, rowNumber, 7, GetCompanyValue(nomination, CompanyValue.Innovation));
                SetCellValue(cells, rowNumber, 8, GetCompanyValue(nomination, CompanyValue.CustomerFocus));
                SetCellValue(cells, rowNumber, 9, GetCompanyValue(nomination, CompanyValue.IndividualIntegrity));
                SetCellValue(cells, rowNumber, 10, GetCompanyValue(nomination, CompanyValue.Performance));
                SetCellValue(cells, rowNumber, 11, nomination.WriteUp.ToString());
                SetCellValue(cells, rowNumber, 12, nomination.NomineeEmailAddress.ToString());

                ++rowNumber;
            }
        }
    }
}