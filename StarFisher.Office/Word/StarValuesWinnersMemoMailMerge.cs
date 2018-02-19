using System;
using Microsoft.Office.Interop.Word;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    internal class StarValuesWinnersMemoMailMerge : MailMergeBase
    {
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly NominationList _nominationList;

        public StarValuesWinnersMemoMailMerge(IExcelFileFactory excelFileFactory, NominationList nominationList)
            : base(@"StarFisher.Office.Word.MailMergeTemplates.StarValuesWinnersMemoMailMergeTemplate.docx", WdMailMergeMainDocType.wdCatalog)
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _nominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
        }

        protected override IExcelFile GetDataSourceExcelFile()
        {
            return _excelFileFactory.GetStarValuesWinnersMemoSourceExcelFile(_nominationList);
        }
    }
}
