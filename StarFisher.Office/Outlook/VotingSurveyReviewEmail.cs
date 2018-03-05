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
            var awardsName = nominationList.AwardsPeriod.AwardsName;
            var hasStarValues = nominationList.HasNominationsForAward(AwardType.StarValues);
            var hasRisingStar = nominationList.HasNominationsForAward(AwardType.RisingStar);
            var hasSuperStar = nominationList.HasNominationsForAward(AwardType.SuperStar);

            mailItem.To = string.Join(";", eiaChairPerson.EmailAddress);
            mailItem.Subject = $@"EIA: {awardsName} voting survey review request";

            var content = CreateContentNode();

            AppendRequest(hasRisingStar, hasStarValues, content, eiaChairPerson, awardsName);

            AddVotingGuideAttachments(com, mailItem, content, mailMergeFactory, nominationList, hasStarValues,
                hasRisingStar, hasSuperStar);

            AppendVotingSurveyWebLink(votingSurveyWebLink, content);
            AppendThanks(content);
            WriteMailItemBody(mailItem, content);
        }

        private static void AddVotingGuideAttachments(ComObjectManager com, MailItem mailItem, HtmlNode content,
            IMailMergeFactory mailMergeFactory, NominationList nominationList, bool hasStarValues, bool hasRisingStar,
            bool hasSuperStar)
        {
            var awardCategory = nominationList.AwardsPeriod.AwardCategory;
            if (awardCategory == AwardCategory.QuarterlyAwards)
            {
                AddQuarterlyVotingGuideAttachments(com, mailItem, content, mailMergeFactory, nominationList,
                    hasStarValues, hasRisingStar);
            }
            else if (awardCategory == AwardCategory.SuperStarAwards)
            {
                AddSuperStarVotingGuideAttachments(com, mailItem, mailMergeFactory, nominationList, hasSuperStar);
            }
        }

        private static void AddSuperStarVotingGuideAttachments(ComObjectManager com, MailItem mailItem,
            IMailMergeFactory mailMergeFactory, NominationList nominationList, bool hasSuperStar)
        {
            if (hasSuperStar)
                AddVotingGuideAttachment(com, mailItem, mailMergeFactory, nominationList, AwardType.RisingStar);
        }

        private static void AddQuarterlyVotingGuideAttachments(ComObjectManager com, MailItem mailItem, HtmlNode content,
            IMailMergeFactory mailMergeFactory, NominationList nominationList, bool hasStarValues, bool hasRisingStar)
        {
            if (!hasStarValues)
                AppendNoNomineesCaveat(content, AwardType.StarValues);
            else
                AddVotingGuideAttachment(com, mailItem, mailMergeFactory, nominationList, AwardType.StarValues);

            if (!hasRisingStar)
                AppendNoNomineesCaveat(content, AwardType.RisingStar);
            else
                AddVotingGuideAttachment(com, mailItem, mailMergeFactory, nominationList, AwardType.RisingStar);
        }

        private static void AddVotingGuideAttachment(ComObjectManager com, MailItem mailItem,
            IMailMergeFactory mailMergeFactory,
            NominationList nominationList, AwardType awardType)
        {
            var attachments = com.Get(() => mailItem.Attachments);
            var fileName = awardType.GetVotingGuideFileName(nominationList.AwardsPeriod);
            var filePath = FilePath.Create(Path.Combine(Path.GetTempPath(), fileName), false);

            if (File.Exists(filePath.Value))
                File.Delete(filePath.Value);

            var mailMerge = mailMergeFactory.GetVotingGuideMailMerge(awardType, nominationList);
            mailMerge.Execute(filePath);

            com.Get(() => attachments.Add(filePath.Value));
        }

        private static void AppendVotingSurveyWebLink(string votingSurveyWebLink, HtmlNode content)
        {
            AppendSection(content, $"Here is the survey link: <a href=\"{votingSurveyWebLink}\">{votingSurveyWebLink}</a><o:p></o:p></p>");
        }

        private static void AppendNoNomineesCaveat(HtmlNode content, AwardType awardType)
        {
            AppendSection(content, $@"We had no eligible {awardType.PrettyName} nominees this time.");
        }

        private static void AppendRequest(bool hasRisingStar, bool hasStarValues, HtmlNode content,
            Person eiaChairPerson,
            string awardsName)
        {
            var guideOrGuides = hasRisingStar && hasStarValues ? @"guides" : "guide";

            AppendParagraph(content, $@"Hi {eiaChairPerson.Name.FirstName},");
            AppendSection(content,
                $@"Could you please review and approve the attached voting {
                        guideOrGuides
                    } and below-linked survey for the {awardsName}?");
        }
    }
}