using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.NominationListAggregate.Entities;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Excel
{
    internal abstract class VotingGuideSourceExcelFileBase : ExcelFileBase
    {
        protected VotingGuideSourceExcelFileBase(IEnumerable<Nomination> nominations)
            : base((com, worksheet) => BuildWorksheet(com,
                nominations ?? throw new ArgumentNullException(nameof(nominations)),
                worksheet))
        {
        }

        private static void BuildWorksheet(ComObjectManager com, IEnumerable<Nomination> nominations,
            Worksheet worksheet)
        {
            var cells = com.Get(() => worksheet.Cells);

            cells.NumberFormat = "@"; // Format all cells as text.

            SetCellValue(cells, 1, 1, @"Record_");
            SetCellValue(cells, 1, 2, @"Nomination_IDs");
            SetCellValue(cells, 1, 3, @"Continuously_Improving");
            SetCellValue(cells, 1, 4, @"Driving_Innovation");
            SetCellValue(cells, 1, 5, @"Delighting_Customers");
            SetCellValue(cells, 1, 6, @"Behaving_with_Integrity");
            SetCellValue(cells, 1, 7, @"Delivering_Meaningful_Outcomes");
            SetCellValue(cells, 1, 8, @"Streaming_Good");
            SetCellValue(cells, 1, 9, @"WRITEUP");

            var rowNumber = 2;
            foreach (var nomination in nominations)
            {
                SetCellValue(cells, rowNumber, 1, rowNumber - 1);
                SetCellValue(cells, rowNumber, 2, nomination.VotingIdentifier.ToString());
                SetCellValue(cells, rowNumber, 3, GetCompanyValue(nomination, CompanyValue.ContinuouslyImproving));
                SetCellValue(cells, rowNumber, 4, GetCompanyValue(nomination, CompanyValue.DrivingInnovation));
                SetCellValue(cells, rowNumber, 5, GetCompanyValue(nomination, CompanyValue.DelightingCustomers));
                SetCellValue(cells, rowNumber, 6, GetCompanyValue(nomination, CompanyValue.BehavingWithIntegrity));
                SetCellValue(cells, rowNumber, 7, GetCompanyValue(nomination, CompanyValue.DeliveringMeaningfulOutcomes));
                SetCellValue(cells, rowNumber, 8, GetCompanyValue(nomination, CompanyValue.StreamingGood));
                SetCellValue(cells, rowNumber, 9, nomination.WriteUp.ToString());

                ++rowNumber;
            }
        }
    }
}