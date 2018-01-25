using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal abstract class VotingGuideSourceExcelFileBase : ExcelFileBase
    {
        protected VotingGuideSourceExcelFileBase(IEnumerable<Nomination> nominations)
            : base((com, worksheet) => BuildWorksheet(com,
                nominations ?? throw new ArgumentNullException(nameof(nominations)),
                worksheet))
        { }

        private static void BuildWorksheet(ComObjectManager com, IEnumerable<Nomination> nominations, Worksheet worksheet)
        {
            var cells = com.Get(() => worksheet.Cells);

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Record_");
            SetCellValue(cells, 1, 2, @"Nomination_IDs");
            SetCellValue(cells, 1, 3, @"Learning_Culture");
            SetCellValue(cells, 1, 4, @"Innovation");
            SetCellValue(cells, 1, 5, @"Customer_Focus");
            SetCellValue(cells, 1, 6, @"Individual_Integrity");
            SetCellValue(cells, 1, 7, @"Performance");
            SetCellValue(cells, 1, 8, @"WRITEUP");

            var rowNumber = 2;
            foreach(var nomination in nominations)
            {
                SetCellValue(cells, rowNumber, 1, nomination.Id);
                SetCellValue(cells, rowNumber, 2, nomination.VotingIdentifier.ToString());
                SetCellValue(cells, rowNumber, 3, GetCompanyValue(nomination, CompanyValue.LearningCulture));
                SetCellValue(cells, rowNumber, 4, GetCompanyValue(nomination, CompanyValue.Innovation));
                SetCellValue(cells, rowNumber, 5, GetCompanyValue(nomination, CompanyValue.CustomerFocus));
                SetCellValue(cells, rowNumber, 6, GetCompanyValue(nomination, CompanyValue.IndividualIntegrity));
                SetCellValue(cells, rowNumber, 7, GetCompanyValue(nomination, CompanyValue.Performance));
                SetCellValue(cells, rowNumber, 8, nomination.WriteUp.ToString());

                ++rowNumber;
            }
        }
    }
}
