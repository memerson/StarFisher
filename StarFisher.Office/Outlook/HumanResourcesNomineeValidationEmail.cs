using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Domain.Utilities;

namespace StarFisher.Office.Outlook
{
    internal class HumanResourcesNomineeValidationEmail : EmailBase
    {
        public HumanResourcesNomineeValidationEmail(IEmailConfiguration emailConfiguration,
            NominationList nominationList)
            : base((com, mailItem) => BuildEmail(
                mailItem,
                emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration)),
                nominationList ?? throw new ArgumentNullException(nameof(nominationList))))
        {
        }

        private static void BuildEmail(MailItem mailItem, IEmailConfiguration emailConfiguration,
            NominationList nominationList)
        {
            mailItem.To = string.Join(";", emailConfiguration.HrPeople.Select(p => p.EmailAddress));
            mailItem.CC = string.Join(";", emailConfiguration.EiaChairPerson.EmailAddress);

            var hasStarValues = nominationList.HasNominationsForAward(AwardType.StarValues);
            var hasRisingStar = nominationList.HasNominationsForAward(AwardType.RisingStar);
            var hasSuperStar = nominationList.HasNominationsForAward(AwardType.SuperStar);
            var awardsName = nominationList.AwardsPeriod.AwardsName;

            mailItem.Subject = $@"Need: {awardsName} nominee eligibility check";

            var content = CreateContentNode();

            AppendIntroduction(content, emailConfiguration.HrPeople, awardsName);
            AppendNomineeCriteria(content, nominationList, hasStarValues, hasRisingStar, hasSuperStar);
            AppendThanks(content);
            AppendNominees(content, nominationList, hasStarValues, hasRisingStar, hasSuperStar);
            WriteMailItemBody(mailItem, content);
        }

        private static void AppendNominees(HtmlNode content, NominationList nominationList, bool hasStarValues,
            bool hasRisingStar, bool hasSuperStar)
        {
            var awardCategory = nominationList.AwardsPeriod.AwardCategory;
            if (awardCategory == AwardCategory.QuarterlyAwards)
                AppendQuarterlyAwardNominees(content, nominationList, hasStarValues, hasRisingStar);
            else if (awardCategory == AwardCategory.SuperStarAwards)
                AppendSuperStarNominees(content, nominationList, hasSuperStar);
        }

        private static void AppendSuperStarNominees(HtmlNode content, NominationList nominationList, bool hasSuperStar)
        {
            if (hasSuperStar)
                AppendNominees(content, nominationList, AwardType.SuperStar);
        }

        private static void AppendQuarterlyAwardNominees(HtmlNode content, NominationList nominationList, bool hasStarValues,
            bool hasRisingStar)
        {
            if (hasStarValues)
                AppendNominees(content, nominationList, AwardType.StarValues);

            if (hasRisingStar)
                AppendNominees(content, nominationList, AwardType.RisingStar);
        }

        private static void AppendNomineeCriteria(HtmlNode content, NominationList nominationList, bool hasStarValues,
            bool hasRisingStar, bool hasSuperStar)
        {
            var awardCategory = nominationList.AwardsPeriod.AwardCategory;
            if (awardCategory == AwardCategory.QuarterlyAwards)
                AppendQuarterlyNomineeCriteria(content, hasStarValues, hasRisingStar);
            else if (awardCategory == AwardCategory.SuperStarAwards)
                AppendSuperStarNomineeCriteria(content, hasSuperStar);
        }

        private static void AppendSuperStarNomineeCriteria(HtmlNode content, bool hasSuperStar)
        {
            if (!hasSuperStar)
                return;

            AppendSection(content, @"Super Star nominees must be full-time employees in good standing. They must have been with HealthStream since January 1.");
        }

        private static void AppendQuarterlyNomineeCriteria(HtmlNode content, bool hasStarValues, bool hasRisingStar)
        {
            if (hasStarValues)
            {
                AppendSection(content, @"Star Values nominees must be full-time or part-time/20 employees in good standing. They must have been with HealthStream for at least one full quarter.");
            }

            if (hasRisingStar)
            {
                AppendSection(content, @"Rising Star nominees must be active interns in good standing who are either in school or within their first year after graduation.");
            }
        }

        private static void AppendIntroduction(HtmlNode content, IReadOnlyList<Person> hrPeople, string awardsName)
        {
            var hrFirstNames = hrPeople.Select(n => n.Name.FirstName).PrettyPrint();

            AppendParagraph(content, $@"Hi {hrFirstNames},");
            AppendSection(content,
                $@"Could you please check the list of nominees for the {
                        awardsName
                    } and let us know if any are not eligible?");
        }

        private static void AppendNominees(HtmlNode content, NominationList nominationList, AwardType awardType)
        {
            var nominationGroups = nominationList.Nominations
                .Where(n => n.AwardType == awardType)
                .GroupBy(n => new { n.NomineeName, n.NomineeOfficeLocation })
                .OrderBy(g => g.Key.NomineeName.FullNameLastNameFirst);

            AppendLineBreak(content);
            AppendSection(content, $@"{awardType}s:");
            AppendLineBreak(content);

            var nomineeTable = AppendTable(content);

            foreach (var group in nominationGroups)
            {
                var nomineeName = group.Key.NomineeName.FullNameLastNameFirst;
                var nomineeOfficeLocation = group.Key.NomineeOfficeLocation.Name;

                var nomineeTableRow = AppendTableRow(nomineeTable);

                AppendTableData(nomineeTableRow, nomineeName);
                AppendTableData(nomineeTableRow, nomineeOfficeLocation);
            }
        }
    }
}