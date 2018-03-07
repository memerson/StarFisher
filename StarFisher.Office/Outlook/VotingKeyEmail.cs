using System;
using System.IO;
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

            var hasStarValues = nominationList.HasNominationsForAward(AwardType.StarValues);
            var hasRisingStar = nominationList.HasNominationsForAward(AwardType.RisingStar);
            var hasSuperStar = nominationList.HasNominationsForAward(AwardType.SuperStar);

            var content = HtmlNode.CreateNode(@"<div class=WordSection1>");

            AppendRequest(hasRisingStar, hasStarValues, content, eiaChairPerson, awardsName);

            AddVotingKeyAttachments(com, mailItem, content, excelFileFactory, nominationList, hasStarValues,
                hasRisingStar, hasSuperStar);

            AppendThanks(content);
            WriteMailItemBody(mailItem, content);
        }

        private static void AddVotingKeyAttachments(ComObjectManager com, MailItem mailItem, HtmlNode content,
            IExcelFileFactory excelFileFactory, NominationList nominationList, bool hasStarValues, bool hasRisingStar,
            bool hasSuperStar)
        {
            var awardCategory = nominationList.AwardsPeriod.AwardCategory;
            if (awardCategory == AwardCategory.QuarterlyAwards)
            {
                AddQuarterlyVotingKeyAttachments(com, mailItem, content, excelFileFactory, nominationList,
                    hasStarValues, hasRisingStar);
            }
            else if (awardCategory == AwardCategory.SuperStarAwards)
            {
                AddSuperStarVotingKeyAttachments(com, mailItem, excelFileFactory, nominationList, hasSuperStar);
            }
        }

        private static void AddSuperStarVotingKeyAttachments(ComObjectManager com, MailItem mailItem,
            IExcelFileFactory excelFileFactory, NominationList nominationList, bool hasSuperStar)
        {
            if (hasSuperStar)
                AddVotingKeyAttachment(com, mailItem, excelFileFactory, nominationList, AwardType.SuperStar);
        }

        private static void AddQuarterlyVotingKeyAttachments(ComObjectManager com, MailItem mailItem, HtmlNode content,
            IExcelFileFactory excelFileFactory, NominationList nominationList, bool hasStarValues, bool hasRisingStar)
        {
            if (!hasStarValues)
                AppendNoNomineesCaveat(content, AwardType.StarValues);
            else
                AddVotingKeyAttachment(com, mailItem, excelFileFactory, nominationList, AwardType.StarValues);

            if (!hasRisingStar)
                AppendNoNomineesCaveat(content, AwardType.RisingStar);
            else
                AddVotingKeyAttachment(com, mailItem, excelFileFactory, nominationList, AwardType.RisingStar);
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

        private static void AppendNoNomineesCaveat(HtmlNode content, AwardType awardType)
        {
            AppendSection(content, $@"We had no eligible {awardType.PrettyName} nominees this time.");
        }

        private static void AppendRequest(bool hasRisingStar, bool hasStarValues, HtmlNode content,
            Person eiaChairPerson, string awardsName)
        {
            var keyorKeys = hasRisingStar && hasStarValues ? @"keys" : "key";

            AppendParagraph(content, $@"Hi {eiaChairPerson.Name.FirstName},");
            AppendSection(content, $@"Please find attached the {awardsName} voting {keyorKeys}.");
        }
    }
}