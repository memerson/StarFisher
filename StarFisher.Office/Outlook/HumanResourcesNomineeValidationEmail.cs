using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.Utilities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Outlook
{
    internal class HumanResourcesNomineeValidationEmail : EmailBase
    {
        internal HumanResourcesNomineeValidationEmail(IEmailConfiguration emailConfiguration, NominationList nominationList)
            : base((com, mailItem) => BuildEmail(
                mailItem, 
                emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration)), 
                nominationList ?? throw new ArgumentNullException(nameof(nominationList))))
        { }

        private static void BuildEmail(MailItem mailItem, IEmailConfiguration emailConfiguration,
            NominationList nominationList)
        {
            mailItem.To = string.Join(";", emailConfiguration.HrPeople.Select(p => p.EmailAddress));
            mailItem.CC = string.Join(";", emailConfiguration.EiaChairPerson.EmailAddress);
            var hasStarValues = nominationList.Nominations.Any(n => n.AwardType == AwardType.StarValues);
            var hasRisingStar = nominationList.Nominations.Any(n => n.AwardType == AwardType.RisingStar);

            mailItem.Subject = $@"Need: {nominationList.Quarter} Star Awards nominee eligibility check";

            var document = new HtmlDocument();
            document.LoadHtml(mailItem.HTMLBody);

            var content = HtmlNode.CreateNode(@"<div class=WordSection1>");

            WriteInstructions(emailConfiguration.HrPeople, nominationList.Quarter, content, hasStarValues,
                hasRisingStar);

            if (hasStarValues)
                WriteNominees(nominationList, AwardType.StarValues, content);

            if(hasRisingStar)
                WriteNominees(nominationList, AwardType.RisingStar, content);

            var body = document.DocumentNode.SelectSingleNode("//body");
            body.ChildNodes.Prepend(content);

            mailItem.HTMLBody = document.DocumentNode.OuterHtml;
        }

        private static void WriteInstructions(IReadOnlyList<Person> hrPeople, Quarter quarter,
            HtmlNode content, bool hasStarValues, bool hasRisingStar)
        {
            var hrFirstNames = hrPeople.Select(n => n.Name.FirstName).PrettyPrint();

            content.ChildNodes.Append(HtmlNode.CreateNode($@"<p class=MsoNormal>Hi {hrFirstNames},</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(
                $@"<p class=MsoNormal>Could you please check the list of nominees for the {quarter} Star Awards and let us know if any are not eligible?</p>"));

            if (hasStarValues)
            {
                content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
                content.ChildNodes.Append(HtmlNode.CreateNode(
                    @"<p class=MsoNormal>Star Values nominees must be full-time or part-time/20 employees in good standing. They must have been with HealthStream for at least one full quarter.</p>"));
            }

            if (hasRisingStar)
            {
                content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
                content.ChildNodes.Append(HtmlNode.CreateNode(
                    @"<p class=MsoNormal>Rising Star nominees must be active interns in good standing who are either in school or within their first year after graduation.</p>"));
            }

            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<p class=MsoNormal>Thanks!</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
        }

        private static void WriteNominees(NominationList nominationList, AwardType awardType, HtmlNode content)
        {
            var nominationGroups = nominationList.Nominations
                .Where(n => n.AwardType == awardType)
                .GroupBy(n => new {n.NomineeName, n.NomineeOfficeLocation})
                .OrderBy(g => g.Key.NomineeName.FullNameLastNameFirst);

            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));
            content.ChildNodes.Append(HtmlNode.CreateNode($@"<p class=MsoNormal>{awardType}s:</p>"));
            content.ChildNodes.Append(HtmlNode.CreateNode(@"<br>"));

            var nomineeTable = HtmlNode.CreateNode(@"<table class=MsoNormalTable>");
            content.ChildNodes.Append(nomineeTable);

            foreach (var group in nominationGroups)
            {
                var nomineeName = group.Key.NomineeName.FullNameLastNameFirst;
                var nomineeOfficeLocation = group.Key.NomineeOfficeLocation.SurveyName;

                var nomineeTableRow = HtmlNode.CreateNode(@"<tr>");
                nomineeTable.AppendChild(nomineeTableRow);

                nomineeTableRow.AppendChild(HtmlNode.CreateNode($@"<td><p class=MsoNormal>{nomineeName}</p></td>"));
                nomineeTableRow.AppendChild(
                    HtmlNode.CreateNode($@"<td><p class=MsoNormal>{nomineeOfficeLocation}</p></td>"));
            }
        }
    }
}
