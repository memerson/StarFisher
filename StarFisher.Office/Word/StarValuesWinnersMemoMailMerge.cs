using System;
using Microsoft.Office.Interop.Word;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    internal class StarValuesWinnersMemoMailMerge : MailMergeBase
    {
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly AwardWinnerList _awardWinnerList;

        public StarValuesWinnersMemoMailMerge(IExcelFileFactory excelFileFactory, AwardWinnerList awardWinnerList)
            : base(@"StarFisher.Office.Word.MailMergeTemplates.StarValuesWinnersMemoMailMergeTemplate.docx", WdMailMergeMainDocType.wdCatalog)
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _awardWinnerList = awardWinnerList ?? throw new ArgumentNullException(nameof(awardWinnerList));
        }

        protected override IExcelFile GetDataSourceExcelFile()
        {
            return _excelFileFactory.GetStarValuesWinnersMemoSourceExcelFile(_awardWinnerList);
        }
    }
}
