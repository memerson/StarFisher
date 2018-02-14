using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.Utilities;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;
using OutlookApplication = Microsoft.Office.Interop.Outlook.Application;

namespace StarFisher.Office.Outlook
{
    internal class HumanResourcesNomineeValidationEmail : IEmail
    {
        private readonly MailItem _mailItem;
        private bool _isDisposed;

        internal HumanResourcesNomineeValidationEmail(IEmailConfiguration emailConfiguration, NominationList nominationList)
        {
            _mailItem = CreateMailItem(emailConfiguration, nominationList);
        }

        private ComObjectManager Com => new ComObjectManager();

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            Com.Dispose();
        }

        public void Display()
        {
            if(_isDisposed)
                throw new ObjectDisposedException("Object is disposed.");

            _mailItem.Display();
        }

        private MailItem CreateMailItem(IEmailConfiguration emailConfiguration,
            NominationList nominationList)
        {
            var outlook = Com.Get(() => new OutlookApplication());
            var mailItem = Com.Get(() => (MailItem)outlook.CreateItem(OlItemType.olMailItem));

            mailItem.Open += (ref bool cancel) =>
            {
                BuildEmail(mailItem, emailConfiguration, nominationList);
            };

            return mailItem;
        }

        private static void BuildEmail(MailItem mailItem, IEmailConfiguration emailConfiguration,
            NominationList nominationList)
        {
            mailItem.To = string.Join(";", emailConfiguration.HrPeople.Select(p => p.EmailAddress));
            mailItem.CC = string.Join(";", emailConfiguration.EiaChairPerson.EmailAddress);

            mailItem.Subject = $@"Need: {nominationList.Quarter} Star Awards nominee eligibility check";

            var hasStarValues = nominationList.Nominations.Any(n => n.AwardType == AwardType.StarValues);
            var hasStarRising = nominationList.Nominations.Any(n => n.AwardType == AwardType.StarRising);

            var document = new HtmlDocument();
            document.LoadHtml(mailItem.HTMLBody);

            var content = HtmlNode.CreateNode(@"<div class=WordSection1>");

            WriteInstructions(emailConfiguration.HrPeople, nominationList.Quarter, content, hasStarValues,
                hasStarRising);

            if (hasStarValues)
                WriteNominees(nominationList, AwardType.StarValues, content);

            if(hasStarRising)
                WriteNominees(nominationList, AwardType.StarRising, content);

            var body = document.DocumentNode.SelectSingleNode("//body");
            body.ChildNodes.Prepend(content);

            mailItem.HTMLBody = document.DocumentNode.OuterHtml;
        }

        private static void WriteInstructions(IReadOnlyList<Person> hrPeople, Quarter quarter,
            HtmlNode content, bool hasStarValues, bool hasStarRising)
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

            if (hasStarRising)
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
                var nomineeName =@group.Key.NomineeName.FullNameLastNameFirst;
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
