using System;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    internal class StarValuesVotingGuideMailMerge : FormLetterMailMergeBase
    {
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly NominationList _nominationList;

        public StarValuesVotingGuideMailMerge(IExcelFileFactory excelFileFactory, NominationList nominationList)
            : base(@"StarFisher.Office.Word.MailMergeTemplates.StarValuesVotingGuideMailMergeTemplate.docx")
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _nominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
        }

        protected override IExcelFile GetDataSourceExcelFile()
        {
            return _excelFileFactory.GetVotingGuideSourceExcelFile(AwardType.StarValues, _nominationList);
        }
    }
}