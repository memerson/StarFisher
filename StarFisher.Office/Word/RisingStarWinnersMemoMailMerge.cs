using System;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    internal class RisingStarWinnersMemoMailMerge : CatalogLetterMailMergeBase
    {
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly NominationList _nominationList;

        public RisingStarWinnersMemoMailMerge(IExcelFileFactory excelFileFactory, NominationList nominationList)
            : base(@"StarFisher.Office.Word.MailMergeTemplates.RisingStarWinnersMemoMailMergeTemplate.docx")
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _nominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
        }

        protected override IExcelFile GetDataSourceExcelFile()
        {
            return _excelFileFactory.GetRisingStarWinnersMemoSourceExcelFile(_nominationList);
        }
    }
}