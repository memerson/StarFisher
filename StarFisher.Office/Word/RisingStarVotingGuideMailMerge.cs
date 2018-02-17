using System;
using Microsoft.Office.Interop.Word;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    public class RisingStarVotingGuideMailMerge : MailMergeBase
    {
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly NominationList _nominationList;

        public RisingStarVotingGuideMailMerge(IExcelFileFactory excelFileFactory, NominationList nominationList)
            : base(@"StarFisher.Office.Word.MailMergeTemplates.RisingStarVotingGuideMailMergeTemplate.docx", WdMailMergeMainDocType.wdFormLetters)
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _nominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
        }

        protected override IExcelFile GetDataSourceExcelFile()
        {
            return _excelFileFactory.GetVotingGuideSourceExcelFile(AwardType.RisingStar, _nominationList);
        }
    }
}