using System;
using Microsoft.Office.Interop.Word;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    public class RisingStarCertificatesMailMerge : MailMergeBase
    {
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly AwardWinnerList _awardWinnerList;

        public RisingStarCertificatesMailMerge(IExcelFileFactory excelFileFactory, AwardWinnerList awardWinnerList)
            : base(@"StarFisher.Office.Word.MailMergeTemplates.RisingStarCertificatesMailMergeTemplate.docx", WdMailMergeMainDocType.wdFormLetters)
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _awardWinnerList = awardWinnerList ?? throw new ArgumentNullException(nameof(awardWinnerList));
        }

        protected override IExcelFile GetDataSourceExcelFile()
        {
            return _excelFileFactory.GetCertificatesSourceExcelFile(AwardType.RisingStar, _awardWinnerList);
        }
    }
}