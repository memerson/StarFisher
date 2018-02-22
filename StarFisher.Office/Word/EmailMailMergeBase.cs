using Microsoft.Office.Interop.Word;
using StarFisher.Office.Outlook;
using StarFisher.Office.Utilities;
using OutlookApplication = Microsoft.Office.Interop.Outlook.Application;

namespace StarFisher.Office.Word
{
    internal abstract class EmailMailMergeBase : MailMergeBase
    {
        protected EmailMailMergeBase(string mailMergeTemplateResourceName)
            : base(mailMergeTemplateResourceName, WdMailMergeMainDocType.wdEMail)
        {
        }

        protected override void DoMergeTypeSetup(ComObjectManager com, MailMerge mailMerge)
        {
            ActivateOutlookWorkOffline(com);
            mailMerge.MailAddressFieldName = @"Email";
            mailMerge.MailFormat = WdMailMergeMailFormat.wdMailFormatHTML;
            mailMerge.MailSubject = GetEmailSubject();
            mailMerge.Destination = WdMailMergeDestination.wdSendToEmail;
        }

        protected abstract string GetEmailAddresFieldName();

        protected abstract string GetEmailSubject();

        private static void ActivateOutlookWorkOffline(ComObjectManager com)
        {
            var outlook = com.Get(() => new OutlookApplication());
            outlook.ActivateWorkOffline(com);
        }
    }
}
