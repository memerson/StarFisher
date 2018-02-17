using System;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Office.Utilities;
using OutlookApplication = Microsoft.Office.Interop.Outlook.Application;

namespace StarFisher.Office.Outlook
{
    public abstract class EmailBase : IEmail
    {
        private readonly MailItem _mailItem;
        private bool _isDisposed;

        protected EmailBase(Action<ComObjectManager, MailItem> buildEmail)
        {
            if (buildEmail == null)
                throw new ArgumentNullException(nameof(buildEmail));

            Com = new ComObjectManager();
            _mailItem = CreateMailItem(buildEmail);
        }

        public virtual void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            Com.Dispose();
        }

        public void Display()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Object is disposed.");

            _mailItem.Display();
        }

        protected ComObjectManager Com { get; }

        private MailItem CreateMailItem(Action<ComObjectManager, MailItem> buildEmail)
        {
            var outlook = Com.Get(() => new OutlookApplication());
            var mailItem = Com.Get(() => (MailItem)outlook.CreateItem(OlItemType.olMailItem));

            mailItem.Open += (ref bool cancel) =>
            {
                buildEmail(Com, mailItem);
            };

            return mailItem;
        }
    }
}
