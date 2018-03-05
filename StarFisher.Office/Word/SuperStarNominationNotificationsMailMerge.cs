using System;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    internal class SuperStarNominationNotificationsMailMerge : EmailMailMergeBase
    {
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly NominationList _nominationList;

        public SuperStarNominationNotificationsMailMerge(IExcelFileFactory excelFileFactory, NominationList nominationList)
            : base(@"StarFisher.Office.Word.MailMergeTemplates.SuperStarNominationNotificationsMailMergeTemplate.docx")
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _nominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
        }

        protected override IExcelFile GetDataSourceExcelFile()
        {
            return _excelFileFactory.GetNominationNotificationEmailSourceExcelFile(AwardType.SuperStar, _nominationList);
        }

        protected override string GetEmailAddresFieldName()
        {
            return @"Email";
        }

        protected override string GetEmailSubject()
        {
            return @"Super Star Award Nomination";
        }
    }
}