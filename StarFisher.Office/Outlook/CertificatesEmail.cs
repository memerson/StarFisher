using System;
using System.IO;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Utilities;
using StarFisher.Office.Word;

namespace StarFisher.Office.Outlook
{
    internal class CertificatesEmail : EmailBase
    {
        public CertificatesEmail(IEmailConfiguration emailConfiguration, IMailMergeFactory mailMergeFactory,
            NominationList nominationList)
            : base((com, mailItem) => BuildEmail(
                com,
                mailItem,
                emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration)),
                mailMergeFactory ?? throw new ArgumentNullException(nameof(mailMergeFactory)),
                nominationList ?? throw new ArgumentNullException(nameof(nominationList))))
        {
        }

        private static void BuildEmail(ComObjectManager com, MailItem mailItem, IEmailConfiguration emailConfiguration,
            IMailMergeFactory mailMergeFactory, NominationList nominationList)
        {
            var certificatePrinter = emailConfiguration.CertificatePrinterPerson;
            var awardsName = nominationList.AwardsPeriod.AwardsName;
            var hasStarValues = nominationList.HasStarValuesAwardWinners;
            var hasRisingStar = nominationList.HasRisingStarAwardWinners;

            mailItem.To = certificatePrinter.EmailAddress.Value;
            mailItem.CC = emailConfiguration.EiaChairPerson.EmailAddress.Value;
            mailItem.Subject = $@"EIA: {awardsName} winner certificates";

            var document = new HtmlDocument();
            document.LoadHtml(mailItem.HTMLBody);

            var content = HtmlNode.CreateNode(@"<div class=WordSection1>");

            WriteRequest(content, certificatePrinter, awardsName);

            if (!hasStarValues)
                WriteNoWinnersCaveat(content, AwardType.StarValues);
            else
                AddCertificatesAttachment(com, mailItem, mailMergeFactory, nominationList, AwardType.StarValues);

            if (!hasRisingStar)
                WriteNoWinnersCaveat(content, AwardType.RisingStar);
            else
                AddCertificatesAttachment(com, mailItem, mailMergeFactory, nominationList, AwardType.RisingStar);

            WriteThanks(content);

            var body = document.DocumentNode.SelectSingleNode("//body");
            body.ChildNodes.Prepend(content);

            mailItem.HTMLBody = document.DocumentNode.OuterHtml;
        }

        private static void AddCertificatesAttachment(ComObjectManager com, MailItem mailItem,
            IMailMergeFactory mailMergeFactory,
            NominationList nominationList, AwardType awardType)
        {
            var attachments = com.Get(() => mailItem.Attachments);
            var fileName = awardType.GetCertificatesFileName(nominationList.AwardsPeriod);
            var filePath = FilePath.Create(Path.Combine(Path.GetTempPath(), fileName), false);

            if (File.Exists(filePath.Value))
                File.Delete(filePath.Value);

            var mailMerge = mailMergeFactory.GetCertificatesMailMerge(awardType, nominationList);
            mailMerge.Execute(filePath);

            com.Get(() => attachments.Add(filePath.Value));
        }

        private static void WriteThanks(HtmlNode content)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<p class=MsoNormal>Thanks!</p>"));
        }

        private static void WriteNoWinnersCaveat(HtmlNode content, AwardType awardType)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>We had no {awardType.PrettyName} winners this time.</p>"));
        }

        private static void WriteRequest(HtmlNode content, Person certificatePrinter, string awardsName)
        {
            content.ChildNodes.Append(
                HtmlNode.CreateNode($@"<p class=MsoNormal>Hi {certificatePrinter.Name.FirstName},</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>Please find attached the {awardsName} winners certificates.</p>"));
        }
    }
}