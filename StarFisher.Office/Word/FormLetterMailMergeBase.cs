using Microsoft.Office.Interop.Word;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Word
{
    internal abstract class FormLetterMailMergeBase : MailMergeBase
    {
        protected FormLetterMailMergeBase(string mailMergeTemplateResourceName)
            : base(mailMergeTemplateResourceName, WdMailMergeMainDocType.wdFormLetters)
        {
        }

        protected override void DoMergeTypeSetup(ComObjectManager com, MailMerge mailMerge)
        {
            mailMerge.Destination = WdMailMergeDestination.wdSendToNewDocument;
        }
    }
}