using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;
using StarFisher.Office.Word;

namespace StarFisher.Office.Outlook
{
    internal class VotingSurveyReviewEmail : EmailBase
    {
        public VotingSurveyReviewEmail(IEmailConfiguration emailConfiguration, IMailMergeFactory mailMergeFactory,
            NominationList nominationList, string votingSurveyWebLink)
            : base((com, mailItem) => BuildEmail(
                com,
                mailItem,
                emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration)),
                mailMergeFactory ?? throw new ArgumentNullException(nameof(mailMergeFactory)),
                nominationList ?? throw new ArgumentNullException(nameof(nominationList)),
                votingSurveyWebLink))
        {
            if (string.IsNullOrWhiteSpace(votingSurveyWebLink))
                throw new ArgumentException(votingSurveyWebLink);
        }

        private static void BuildEmail(ComObjectManager com, MailItem mailItem, IEmailConfiguration emailConfiguration,
            IMailMergeFactory mailMergeFactory, NominationList nominationList, string votingSurveyWebLink)
        {
            var eiaChairPerson = emailConfiguration.EiaChairPerson;
            var quarter = nominationList.Quarter.Abbreviation;
            var hasStarValues = nominationList.Nominations.Any(n => n.AwardType == AwardType.StarValues);
            var hasRisingStar = nominationList.Nominations.Any(n => n.AwardType == AwardType.RisingStar);

            mailItem.To = string.Join(";", eiaChairPerson.EmailAddress);
            mailItem.Subject = $@"EIA: {quarter} Star Awards voting survey review request";

            var document = new HtmlDocument();
            document.LoadHtml(mailItem.HTMLBody);

            var content = HtmlNode.CreateNode(@"<div class=WordSection1>");

            WriteRequest(hasRisingStar, hasStarValues, content, eiaChairPerson, quarter);

            if (!hasStarValues)
                WriteNoNomineesCaveat(content, AwardType.StarValues, quarter);
            else
                AddVotingGuideAttachment(com, mailItem, mailMergeFactory, nominationList, AwardType.StarValues);

            if (!hasRisingStar)
                WriteNoNomineesCaveat(content, AwardType.RisingStar, quarter);
            else
                AddVotingGuideAttachment(com, mailItem, mailMergeFactory, nominationList, AwardType.RisingStar);

            WriteVotingSurveyWebLink(votingSurveyWebLink, content);
            WriteThanks(content);

            var body = document.DocumentNode.SelectSingleNode("//body");
            body.ChildNodes.Prepend(content);

            mailItem.HTMLBody = document.DocumentNode.OuterHtml;
        }

        private static void AddVotingGuideAttachment(ComObjectManager com, MailItem mailItem,
            IMailMergeFactory mailMergeFactory,
            NominationList nominationList, AwardType awardType)
        {
            var attachments = com.Get(() => mailItem.Attachments);
            var fileName = awardType.GetVotingGuideFileName(nominationList.Year, nominationList.Quarter);
            var filePath = FilePath.Create(Path.Combine(Path.GetTempPath(), fileName), false);

            if (File.Exists(filePath.Value))
                File.Delete(filePath.Value);

            var mailMerge = mailMergeFactory.GetVotingGuideMailMerge(awardType, nominationList);
            mailMerge.Execute(filePath);

            com.Get(() => attachments.Add(filePath.Value));
        }

        private static void WriteThanks(HtmlNode content)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<p class=MsoNormal>Thanks!</p>"));
        }

        private static void WriteVotingSurveyWebLink(string votingSurveyWebLink, HtmlNode content)
        {
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $"<p class=MsoNormal>Here is the survey link: <a href=\"{votingSurveyWebLink}\">{votingSurveyWebLink}</a><o:p></o:p></p>"));
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
            var guideOrGuides = hasRisingStar && hasStarValues ? @"guides" : "guide";

            content.ChildNodes.Append(
                HtmlNode.CreateNode($@"<p class=MsoNormal>Hi {eiaChairPerson.Name.FirstName},</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>Could you please review and approve the attached voting {
                        guideOrGuides
                    } and below-linked survey for the {quarter} Star Awards?</p>"));
        }
    }
}