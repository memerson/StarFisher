using System;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.Utilities;
using StarFisher.Office.Utilities;
using OutlookApplication = Microsoft.Office.Interop.Outlook.Application;

namespace StarFisher.Office.Outlook
{
    internal class HumanResourcesNomineeValidationEmail : IEmail
    {
        private readonly MailItem _mailItem;
        private bool _isDisposed;

        internal HumanResourcesNomineeValidationEmail(IEmailConfiguration emailConfiguration, NominationList nominationList)
        {
            _mailItem = CreateMailItem(emailConfiguration, nominationList);
        }

        private ComObjectManager Com => new ComObjectManager();

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            Com.Dispose();
        }

        public void Display()
        {
            if(_isDisposed)
                throw new ObjectDisposedException("Object is disposed.");

            _mailItem.Display();
        }

        private MailItem CreateMailItem(IEmailConfiguration emailConfiguration,
            NominationList nominationList)
        {
            var outlook = Com.Get(() => new OutlookApplication());
            var mailItem = Com.Get(() => (MailItem)outlook.CreateItem(OlItemType.olMailItem));

            mailItem.Open += (ref bool cancel) =>
            {
                BuildEmail(mailItem, emailConfiguration, nominationList);
            };

            return mailItem;
        }

        private static void BuildEmail(MailItem mailItem, IEmailConfiguration emailConfiguration,
            NominationList nominationList)
        {
            mailItem.To = string.Join(";", emailConfiguration.HumanResourcesEmailAddresses);
            mailItem.CC = string.Join(";", emailConfiguration.EiaChairPersonEmailAddresses);

            mailItem.Subject = $"Need: {nominationList.Quarter} Star Awards and Rising Star Awards nominee eligibility check";

            var nominationGroups = nominationList.Nominations
                .GroupBy(n => new {n.NomineeName, n.NomineeOfficeLocation})
                .OrderBy(g => g.Key.NomineeName.FullNameLastNameFirst);

            var hrFirstNames = emailConfiguration.HumanResourcesPersonNames
                .Select(n => n.FirstName)
                .PrettyPrint();

            var document = new HtmlDocument();
            document.LoadHtml(mailItem.HTMLBody);

            var content = HtmlNode.CreateNode(@"<div class=WordSection1>");

            content.ChildNodes.Append(HtmlNode.CreateNode($@"<p class=MsoNormal>Hi {hrFirstNames},</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode("<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                @"<p class=MsoNormal>Could you please check the list of nominees for the {nominationList.Quarter} Star Awards and Rising Star Awards and let us know if any are not eligible?</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode("<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode("<p class=MsoNormal>Thanks!</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode("<br>"));

            var nomineeTable = HtmlNode.CreateNode("<table class=MsoNormalTable>");
            content.ChildNodes.Append(nomineeTable);

            foreach (var group in nominationGroups)
            {
                var nomineeName = group.Key.NomineeName.FullNameLastNameFirst;
                var nomineeOfficeLocation = group.Key.NomineeOfficeLocation.Value;

                var nomineeTableRow = HtmlNode.CreateNode("<tr>");
                nomineeTable.AppendChild(nomineeTableRow);

                nomineeTableRow.AppendChild(HtmlNode.CreateNode($"<td><p class=MsoNormal>{nomineeName}</p></td>"));
                nomineeTableRow.AppendChild(
                    HtmlNode.CreateNode($"<td><p class=MsoNormal>{nomineeOfficeLocation}</p></td>"));
            }

            var body = document.DocumentNode.SelectSingleNode("//body");
            body.ChildNodes.Prepend(content);

            mailItem.HTMLBody = document.DocumentNode.OuterHtml;
        }
    }
}
