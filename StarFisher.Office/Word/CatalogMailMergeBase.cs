using Microsoft.Office.Interop.Word;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Word
{
    internal abstract class CatalogLetterMailMergeBase : MailMergeBase
    {
        protected CatalogLetterMailMergeBase(string mailMergeTemplateResourceName)
            : base(mailMergeTemplateResourceName, WdMailMergeMainDocType.wdFormLetters)
        {
        }

        protected override void DoMergeTypeSetup(ComObjectManager com, MailMerge mailMerge)
        {
            mailMerge.Destination = WdMailMergeDestination.wdSendToNewDocument;
        }
    }
}