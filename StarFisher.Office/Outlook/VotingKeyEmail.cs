using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
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
            var quarter = nominationList.Quarter.Abbreviation;

            mailItem.To = string.Join(";", eiaChairPerson.EmailAddress);
            mailItem.Subject = $@"EIA: {quarter} Star Awards voting key";

            var hasStarValues = nominationList.Nominations.Any(n => n.AwardType == AwardType.StarValues);
            var hasRisingStar = nominationList.Nominations.Any(n => n.AwardType == AwardType.RisingStar);

            var document = new HtmlDocument();
            document.LoadHtml(mailItem.HTMLBody);

            var content = HtmlNode.CreateNode(@"<div class=WordSection1>");

            WriteRequest(hasRisingStar, hasStarValues, content, eiaChairPerson, quarter);

            if (!hasStarValues)
                WriteNoNomineesCaveat(content, AwardType.StarValues, quarter);
            else
                AddVotingKeyAttachment(com, mailItem, excelFileFactory, nominationList, AwardType.StarValues);

            if (!hasRisingStar)
                WriteNoNomineesCaveat(content, AwardType.RisingStar, quarter);
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
            var fileName = awardType.GetVotingKeyFileName(nominationList.Year, nominationList.Quarter);
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

        private static void WriteNoNomineesCaveat(HtmlNode content, AwardType awardType, string quarter)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>We had no eligible {awardType.PrettyName} nominees for {quarter}.</p>"));
        }

        private static void WriteRequest(bool hasRisingStar, bool hasStarValues, HtmlNode content,
            Person eiaChairPerson,
            string quarter)
        {
            var keyorKeys = hasRisingStar && hasStarValues ? @"keys" : "key";

            content.ChildNodes.Append(
                HtmlNode.CreateNode($@"<p class=MsoNormal>Hi {eiaChairPerson.Name.FirstName},</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>Please find attached the {quarter} Star Awards voting {keyorKeys}.</p>"));
        }
    }
}