using System;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    internal class StarValuesNominationNotificationsMailMerge : EmailMailMergeBase
    {
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly NominationList _nominationList;

        public StarValuesNominationNotificationsMailMerge(IExcelFileFactory excelFileFactory, NominationList nominationList)
            : base(@"StarFisher.Office.Word.MailMergeTemplates.StarValuesNominationNotificationsMailMergeTemplate.docx")
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _nominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
        }

        protected override IExcelFile GetDataSourceExcelFile()
        {
            return _excelFileFactory.GetNominationNotificationEmailSourceExcelFile(AwardType.StarValues, _nominationList);
        }

        protected override string GetEmailAddresFieldName()
        {
            return @"Name";
        }

        protected override string GetEmailSubject()
        {
            return $@"Star Values Award Nomination";
        }
    }
}