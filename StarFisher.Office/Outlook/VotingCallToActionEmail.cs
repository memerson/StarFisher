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
    internal class VotingCallToActionEmail : EmailBase
    {
        public VotingCallToActionEmail(IMailMergeFactory mailMergeFactory, NominationList nominationList, string votingSurveyWebLink, DateTime votingDeadline)
            : base((com, mailItem) => BuildEmail(
                com,
                mailItem,
                mailMergeFactory ?? throw new ArgumentNullException(nameof(mailMergeFactory)),
                nominationList ?? throw new ArgumentNullException(nameof(nominationList)),
                votingSurveyWebLink,
                votingDeadline))
        {
            if (string.IsNullOrWhiteSpace(votingSurveyWebLink))
                throw new ArgumentException(nameof(votingSurveyWebLink));

            if(votingDeadline == default(DateTime))
                throw new ArgumentException(nameof(votingDeadline));
        }

        private static void BuildEmail(ComObjectManager com, MailItem mailItem,
            IMailMergeFactory mailMergeFactory, NominationList nominationList, string votingSurveyWebLink, DateTime votingDeadline)
        {
            var awardsName = nominationList.AwardsPeriod.AwardsName;
            var hasStarValues = nominationList.HasNominationsForAward(AwardType.StarValues);
            var hasRisingStar = nominationList.HasNominationsForAward(AwardType.RisingStar);
            var hasSuperStar = nominationList.HasNominationsForAward(AwardType.SuperStar);

            mailItem.To = @"#ExcellenceInAction@healthstream.com";
            mailItem.Subject = $@"*** Voting Open for the {awardsName} ***";

            var content = CreateContentNode();

            AppendRequest(content, awardsName, votingDeadline);

            AddVotingGuideAttachmentsAndInstructions(com, mailItem, content, mailMergeFactory, nominationList, hasStarValues,
                hasRisingStar, hasSuperStar);

            AppendVotingSurveyWebLink(votingSurveyWebLink, content);
            AppendThanks(content);
            WriteMailItemBody(mailItem, content);
        }

        private static void AddVotingGuideAttachmentsAndInstructions(ComObjectManager com, MailItem mailItem, HtmlNode content,
            IMailMergeFactory mailMergeFactory, NominationList nominationList, bool hasStarValues, bool hasRisingStar,
            bool hasSuperStar)
        {
            var awardCategory = nominationList.AwardsPeriod.AwardCategory;
            if (awardCategory == AwardCategory.QuarterlyAwards)
            {
                AddQuarterlyVotingGuideAttachmentsAndInstructions(com, mailItem, content, mailMergeFactory, nominationList,
                    hasStarValues, hasRisingStar);
            }
            else if (awardCategory == AwardCategory.SuperStarAwards)
            {
                AddSuperStarVotingGuideAttachmentsAndInstructions(com, mailItem, content, mailMergeFactory, nominationList, hasSuperStar);
            }
        }

        private static void AddSuperStarVotingGuideAttachmentsAndInstructions(ComObjectManager com, MailItem mailItem,
            HtmlNode content, IMailMergeFactory mailMergeFactory, NominationList nominationList, bool hasSuperStar)
        {
            if (!hasSuperStar)
                return;

            var instructions =
                $@"this document has all the nominations for the {
                        AwardType.SuperStar.PrettyName
                    }. Question #5 of the voting survey covers these nominations. The nominees are from all offices and also include remote employees. The survey does NOT list them in any particular order. Please note some nominees may have been nominated multiple times.";

            AddVotingGuideAttachmentAndInstructions(com, mailItem, content, mailMergeFactory, nominationList,
                AwardType.SuperStar, instructions);
        }

        private static void AddQuarterlyVotingGuideAttachmentsAndInstructions(ComObjectManager com, MailItem mailItem, HtmlNode content,
            IMailMergeFactory mailMergeFactory, NominationList nominationList, bool hasStarValues, bool hasRisingStar)
        {
            if (hasStarValues)
                AddStarValuesVotingGuideAttachmentsAndInstructions(com, mailItem, content, mailMergeFactory, nominationList);

            if (hasRisingStar)
                AddRisingStarVotingGuideAttachmentsAndInstructions(com, mailItem, content, mailMergeFactory, nominationList);
        }

        private static void AddRisingStarVotingGuideAttachmentsAndInstructions(ComObjectManager com, MailItem mailItem,
            HtmlNode content, IMailMergeFactory mailMergeFactory, NominationList nominationList)
        {
            var instructions =
                $@"This document has all the nomination for the {
                        AwardType.RisingStar.PrettyName
                    } for interns. Question #3 of the voting survey covers these nominations. The intern nominations are from across the company. The survey does NOT list them in any particular order. Please note some nominees may have been nominated multiple times.";

            AddVotingGuideAttachmentAndInstructions(com, mailItem, content, mailMergeFactory, nominationList,
                AwardType.RisingStar, instructions);
        }

        private static void AddStarValuesVotingGuideAttachmentsAndInstructions(ComObjectManager com, MailItem mailItem,
            HtmlNode content, IMailMergeFactory mailMergeFactory, NominationList nominationList)
        {
            var instructions =
                $@"This document has all the nominations for the {
                        AwardType.StarValues.PrettyName
                    }. Question #2 of the voting survey covers these nominations. The nominees are from all offices and also include remote employees. The survey does NOT list them in any particular order. Please note some nominees may have been nominated multiple times.";

            AddVotingGuideAttachmentAndInstructions(com, mailItem, content, mailMergeFactory, nominationList,
                AwardType.StarValues, instructions);
        }

        private static void AddVotingGuideAttachmentAndInstructions(ComObjectManager com, MailItem mailItem,
            HtmlNode content, IMailMergeFactory mailMergeFactory, NominationList nominationList, AwardType awardType,
            string instructions)
        {
            var attachments = com.Get(() => mailItem.Attachments);
            var fileName = awardType.GetVotingGuideFileName(nominationList.AwardsPeriod);
            var filePath = FilePath.Create(Path.Combine(Path.GetTempPath(), fileName), false);

            if (File.Exists(filePath.Value))
                File.Delete(filePath.Value);

            var mailMerge = mailMergeFactory.GetVotingGuideMailMerge(awardType, nominationList);
            mailMerge.Execute(filePath);

            com.Get(() => attachments.Add(filePath.Value));

            AppendSection(content, $@"<b><i>{awardType.PrettyName} Nominees:</i></b> {fileName} -- {instructions}");
        }

        private static void AppendVotingSurveyWebLink(string votingSurveyWebLink, HtmlNode content)
        {
            AppendSection(content, $"<b><i>Vote today:</i></b> <a href=\"{votingSurveyWebLink}\">{votingSurveyWebLink}</a>");
        }

        private static void AppendRequest(HtmlNode content, string awardsName, DateTime votingDeadline)
        {
            AppendParagraph(content, @"Hello Team,");
            AppendSection(content, $@"It's time to vote for the {awardsName}.");
            AppendSection(content, $@"Please complete voting by <b>{votingDeadline:dddd, MMMM dd}</b>.");
            AppendSection(content, @"You can find the nominations in the attached file(s).");
        }
    }
}