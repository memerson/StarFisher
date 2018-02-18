using System;
using System.IO;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;
using StarFisher.Office.Word;

namespace StarFisher.Office.Outlook
{
    internal class CertificatesEmail : EmailBase
    {
        public CertificatesEmail(IEmailConfiguration emailConfiguration, IMailMergeFactory mailMergeFactory,
            AwardWinnerList awardWinnerList)
            : base((com, mailItem) => BuildEmail(
                com,
                mailItem,
                emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration)),
                mailMergeFactory ?? throw new ArgumentNullException(nameof(mailMergeFactory)),
                awardWinnerList ?? throw new ArgumentNullException(nameof(awardWinnerList))))
        { }

        private static void BuildEmail(ComObjectManager com, MailItem mailItem, IEmailConfiguration emailConfiguration,
            IMailMergeFactory mailMergeFactory, AwardWinnerList awardWinnerList)
        {
            var certificatePrinter = emailConfiguration.CertificatePrinterPerson;
            var quarter = awardWinnerList.Quarter.Abbreviation;
            var hasStarValues = awardWinnerList.RisingStarAwardWinners.Count > 0;
            var hasRisingStar = awardWinnerList.StarValuesAwardWinners.Count > 0;

            mailItem.To = certificatePrinter.EmailAddress.Value;
            mailItem.CC = emailConfiguration.EiaChairPerson.EmailAddress.Value;
            mailItem.Subject = $@"EIA: {quarter} Star Awards winner certificates";

            var document = new HtmlDocument();
            document.LoadHtml(mailItem.HTMLBody);

            var content = HtmlNode.CreateNode(@"<div class=WordSection1>");

            WriteRequest(content, certificatePrinter, quarter);

            if (!hasStarValues)
                WriteNoNomineesCaveat(content, AwardType.StarValues, quarter);
            else
                AddCertificatesAttachment(com, mailItem, mailMergeFactory, awardWinnerList, AwardType.StarValues);

            if (!hasRisingStar)
                WriteNoNomineesCaveat(content, AwardType.RisingStar, quarter);
            else
                AddCertificatesAttachment(com, mailItem, mailMergeFactory, awardWinnerList, AwardType.RisingStar);

            WriteThanks(content);

            var body = document.DocumentNode.SelectSingleNode("//body");
            body.ChildNodes.Prepend(content);

            mailItem.HTMLBody = document.DocumentNode.OuterHtml;
        }

        private static void AddCertificatesAttachment(ComObjectManager com, MailItem mailItem, IMailMergeFactory mailMergeFactory,
            AwardWinnerList awardWinnerList, AwardType awardType)
        {
            var attachments = com.Get(() => mailItem.Attachments);
            var fileName = awardType.GetCertificatesFileName(awardWinnerList.Year, awardWinnerList.Quarter);
            var filePath = FilePath.Create(Path.Combine(Path.GetTempPath(), fileName), false);

            if (File.Exists(filePath.Value))
                File.Delete(filePath.Value);

            var mailMerge = mailMergeFactory.GetCertificatesMailMerge(awardType, awardWinnerList);
            mailMerge.Execute(filePath);

            com.Get(() => attachments.Add(filePath.Value));
        }

        private static void WriteThanks(HtmlNode content)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<p class=MsoNormal>Thanks!</p>"));
        }

        private static void WriteNoNomineesCaveat(HtmlNode content, AwardType awardType, string quarter)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>We had no {awardType.PrettyName} winners for {quarter}.</p>"));
        }

        private static void WriteRequest(HtmlNode content, Person certificatePrinter, string quarter)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode($@"<p class=MsoNormal>Hi {certificatePrinter.Name.FirstName},</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>Please find attached the {quarter} Star Awards winners certificates.</p>"));
        }
    }
}
