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

            mailItem.To = certificatePrinter.EmailAddress.Value;
            mailItem.CC = string.Join(";", emailConfiguration.EiaCoChair1.EmailAddress, emailConfiguration.EiaCoChair2.EmailAddress);
            mailItem.Subject = $@"EIA: {awardsName} winner certificates";

            var content = CreateContentNode();

            AppendIntroduction(content, certificatePrinter, awardsName);
            AddCertificatesAttachments(com, mailItem, mailMergeFactory, nominationList, content);
            AppendThanks(content);

            WriteMailItemBody(mailItem, content);
        }

        private static void AddCertificatesAttachments(ComObjectManager com, MailItem mailItem,
            IMailMergeFactory mailMergeFactory,
            NominationList nominationList, HtmlNode content)
        {
            var awardCategory = nominationList.AwardsPeriod.AwardCategory;

            if (awardCategory == AwardCategory.QuarterlyAwards)
                AddQuarterlyCertificatesAttachments(com, mailItem, mailMergeFactory, nominationList, content);
            else if (awardCategory == AwardCategory.SuperStarAwards)
                AddSuperStarCertificatesAttachments(com, mailItem, mailMergeFactory, nominationList);
        }

        private static void AddSuperStarCertificatesAttachments(ComObjectManager com, MailItem mailItem,
            IMailMergeFactory mailMergeFactory, NominationList nominationList)
        {
            if (!nominationList.HasSuperStarCertificateRecipients)
                return;

            AddCertificatesAttachment(com, mailItem, mailMergeFactory, nominationList, AwardType.SuperStar);
        }

        private static void AddQuarterlyCertificatesAttachments(ComObjectManager com, MailItem mailItem,
            IMailMergeFactory mailMergeFactory, NominationList nominationList, HtmlNode content)
        {
            if (!nominationList.HasStarValuesCertificateRecipients)
                AppendNoCertificateRecipientsCaveat(content, AwardType.StarValues);
            else
                AddCertificatesAttachment(com, mailItem, mailMergeFactory, nominationList, AwardType.StarValues);

            if (!nominationList.HasRisingStarCertificateRecipients)
                AppendNoCertificateRecipientsCaveat(content, AwardType.RisingStar);
            else
                AddCertificatesAttachment(com, mailItem, mailMergeFactory, nominationList, AwardType.RisingStar);
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

        private static void AppendNoCertificateRecipientsCaveat(HtmlNode content, AwardType awardType)
        {
            AppendSection(content, $@"We had no {awardType.PrettyName} certificates to print this time.");
        }

        private static void AppendIntroduction(HtmlNode content, Person certificatePrinter, string awardsName)
        {
            AppendParagraph(content, $@"Hi {certificatePrinter.Name.FirstName},");
            AppendSection(content, $@"Please find attached the {awardsName} winners certificates.");
        }
    }
}