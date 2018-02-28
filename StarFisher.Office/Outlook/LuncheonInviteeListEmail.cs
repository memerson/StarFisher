using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Domain.Utilities;
using StarFisher.Office.Excel;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Outlook
{
    internal class LuncheonInviteeListEmail : EmailBase
    {
        public LuncheonInviteeListEmail(IEmailConfiguration emailConfiguration, IExcelFileFactory excelFileFactory,
            NominationList nominationList)
            : base((com, mailItem) => BuildEmail(
                com,
                mailItem,
                emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration)),
                excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory)),
                nominationList ?? throw new ArgumentNullException(nameof(nominationList))))
        {
        }

        private static void BuildEmail(ComObjectManager com, MailItem mailItem, IEmailConfiguration emailConfiguration,
            IExcelFileFactory excelFileFactory, NominationList nominationList)
        {
            var luncheonPlanners = emailConfiguration.LuncheonPlannerPeople;
            var awardsName = nominationList.AwardsPeriod.AwardsName;

            mailItem.To = string.Join(";", luncheonPlanners.Select(p => p.EmailAddress));
            mailItem.CC = emailConfiguration.EiaChairPerson.EmailAddress.Value;
            mailItem.Subject = $@"EIA: {awardsName} luncheon invite list";

            var document = new HtmlDocument();
            document.LoadHtml(mailItem.HTMLBody);

            var content = HtmlNode.CreateNode(@"<div class=WordSection1>");

            WriteRequest(content, luncheonPlanners, awardsName);

            WriteThanks(content);

            var body = document.DocumentNode.SelectSingleNode("//body");
            body.ChildNodes.Prepend(content);

            mailItem.HTMLBody = document.DocumentNode.OuterHtml;
            AddAwardLuncheonInviteeListAttachment(com, mailItem, excelFileFactory, nominationList);
        }

        private static void AddAwardLuncheonInviteeListAttachment(ComObjectManager com, MailItem mailItem,
            IExcelFileFactory excelFileFactory,
            NominationList nominationList)
        {
            var attachments = com.Get(() => mailItem.Attachments);
            var fileName =
                $@"{nominationList.AwardsPeriod.FileNamePrefix}_StarAwards_LuncheonInvitees.xlsx";
            var filePath = FilePath.Create(Path.Combine(Path.GetTempPath(), fileName), false);

            if (File.Exists(filePath.Value))
                File.Delete(filePath.Value);

            using (var excelFile = excelFileFactory.GetAwardsLuncheonInviteeListExcelFile(nominationList))
            {
                excelFile.Save(filePath);
            }

            com.Get(() => attachments.Add(filePath.Value));
        }

        private static void WriteThanks(HtmlNode content)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<p class=MsoNormal>Thanks!</p>"));
        }

        private static void WriteRequest(HtmlNode content, IEnumerable<Person> luncheonPlanners, string awardsName)
        {
            var luncheonPlannerFirstNames = luncheonPlanners.Select(n => n.Name.FirstName).PrettyPrint();
            content.ChildNodes.Append(HtmlNode.CreateNode($@"<p class=MsoNormal>Hi {luncheonPlannerFirstNames},</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>Please find attached the spreadsheet with the invite list for the {
                        awardsName
                    } luncheon.</p>"));
        }
    }
}