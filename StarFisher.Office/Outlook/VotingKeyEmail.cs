using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Excel;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Outlook
{
    internal class VotingKeyEmail : EmailBase
    {
        public VotingKeyEmail(IEmailConfiguration emailConfiguration, IExcelFileFactory excelFileFactory,
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
            var eiaChairPerson = emailConfiguration.EiaChairPerson;
            var awardsName = nominationList.AwardsPeriod.AwardsName;

            mailItem.To = string.Join(";", eiaChairPerson.EmailAddress);
            mailItem.Subject = $@"EIA: {awardsName} voting key";

            var hasStarValues = nominationList.Nominations.Any(n => n.AwardType == AwardType.StarValues);
            var hasRisingStar = nominationList.Nominations.Any(n => n.AwardType == AwardType.RisingStar);

            var document = new HtmlDocument();
            document.LoadHtml(mailItem.HTMLBody);

            var content = HtmlNode.CreateNode(@"<div class=WordSection1>");

            WriteRequest(hasRisingStar, hasStarValues, content, eiaChairPerson, awardsName);

            if (!hasStarValues)
                WriteNoNomineesCaveat(content, AwardType.StarValues);
            else
                AddVotingKeyAttachment(com, mailItem, excelFileFactory, nominationList, AwardType.StarValues);

            if (!hasRisingStar)
                WriteNoNomineesCaveat(content, AwardType.RisingStar);
            else
                AddVotingKeyAttachment(com, mailItem, excelFileFactory, nominationList, AwardType.RisingStar);

            WriteThanks(content);

            var body = document.DocumentNode.SelectSingleNode("//body");
            body.ChildNodes.Prepend(content);

            mailItem.HTMLBody = document.DocumentNode.OuterHtml;
        }

        private static void AddVotingKeyAttachment(ComObjectManager com, MailItem mailItem,
            IExcelFileFactory excelFileFactory,
            NominationList nominationList, AwardType awardType)
        {
            var attachments = com.Get(() => mailItem.Attachments);
            var fileName = awardType.GetVotingKeyFileName(nominationList.AwardsPeriod);
            var filePath = FilePath.Create(Path.Combine(Path.GetTempPath(), fileName), false);

            if (File.Exists(filePath.Value))
                File.Delete(filePath.Value);

            using (var excelFile = excelFileFactory.GetVotingKeyExcelFile(awardType, nominationList))
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

        private static void WriteNoNomineesCaveat(HtmlNode content, AwardType awardType)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>We had no eligible {awardType.PrettyName} nominees this time.</p>"));
        }

        private static void WriteRequest(bool hasRisingStar, bool hasStarValues, HtmlNode content,
            Person eiaChairPerson,
            string awardsName)
        {
            var keyorKeys = hasRisingStar && hasStarValues ? @"keys" : "key";

            content.ChildNodes.Append(
                HtmlNode.CreateNode($@"<p class=MsoNormal>Hi {eiaChairPerson.Name.FirstName},</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>Please find attached the {awardsName} voting {keyorKeys}.</p>"));
        }
    }
}