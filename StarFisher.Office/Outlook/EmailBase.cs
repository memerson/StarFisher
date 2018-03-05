using System;
using HtmlAgilityPack;
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

        protected ComObjectManager Com { get; }

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

        private MailItem CreateMailItem(Action<ComObjectManager, MailItem> buildEmail)
        {
            var outlook = Com.Get(() => new OutlookApplication());
            var mailItem = Com.Get(() => (MailItem) outlook.CreateItem(OlItemType.olMailItem));

            mailItem.Open += (ref bool cancel) => { buildEmail(Com, mailItem); };

            return mailItem;
        }

        protected static HtmlNode CreateContentNode()
        {
            return HtmlNode.CreateNode(@"<div class=WordSection1>");
        }

        protected static void AppendLineBreak(HtmlNode content)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
        }

        protected static void AppendParagraph(HtmlNode content, string text)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>{text}</p>"));
        }

        protected static void AppendSection(HtmlNode content, string text)
        {
            AppendLineBreak(content);
            AppendParagraph(content, text);
        }

        protected static HtmlNode AppendTable(HtmlNode content)
        {
            var table = HtmlNode.CreateNode(@"<table class=MsoNormalTable>");
            content.ChildNodes.Append(table);
            return table;
        }

        protected static HtmlNode AppendTableRow(HtmlNode table)
        {
            var nomineeTableRow = HtmlNode.CreateNode(@"<tr>");
            table.AppendChild(nomineeTableRow);
            return nomineeTableRow;
        }

        protected static void AppendTableData(HtmlNode tableRow, string text)
        {
            tableRow.AppendChild(HtmlNode.CreateNode($@"<td><p class=MsoNormal>{text}</p></td>"));
        }

        protected static void AppendThanks(HtmlNode content)
        {
            AppendSection(content, @"Thanks!");
        }

        protected static void WriteMailItemBody(MailItem mailItem, HtmlNode content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(mailItem.HTMLBody);

            var body = document.DocumentNode.SelectSingleNode("//body");
            body.ChildNodes.Prepend(content);

            mailItem.HTMLBody = document.DocumentNode.OuterHtml;
        }
    }
}