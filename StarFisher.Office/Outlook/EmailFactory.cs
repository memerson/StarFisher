using System;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Office.Excel;
using StarFisher.Office.Word;

namespace StarFisher.Office.Outlook
{
    public interface IEmailFactory
    {
        IEmail GetHumanResourcesNomineeValidationEmail(NominationList nominationList);
        IEmail GetVotingSurveyReviewEmail(NominationList nominationList, string votingSurveyWebLink);
        IEmail GetVotingKeyEmail(NominationList nominationList);
        IEmail GetLuncheonInviteeListEmail(NominationList nominationList);
    }

    public class EmailFactory : IEmailFactory
    {
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly IMailMergeFactory _mailMergeFactory;

        public EmailFactory(IEmailConfiguration emailConfiguration, IExcelFileFactory excelFileFactory, IMailMergeFactory mailMergeFactory)
        {
            _emailConfiguration = emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration));
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _mailMergeFactory = mailMergeFactory ?? throw new ArgumentNullException(nameof(mailMergeFactory));
        }

        public IEmail GetHumanResourcesNomineeValidationEmail(NominationList nominationList)
        {
            return new HumanResourcesNomineeValidationEmail(_emailConfiguration, nominationList);
        }

        public IEmail GetVotingSurveyReviewEmail(NominationList nominationList, string votingSurveyWebLink)
        {
            return new VotingSurveyReviewEmail(_emailConfiguration, _mailMergeFactory, nominationList, votingSurveyWebLink);
        }

        public IEmail GetVotingKeyEmail(NominationList nominationList)
        {
            return new VotingKeyEmail(_emailConfiguration, _excelFileFactory, nominationList);
        }

        public IEmail GetLuncheonInviteeListEmail(NominationList nominationList)
        {
            return new LuncheonInviteeListEmail(_emailConfiguration, _excelFileFactory, nominationList);
        }
    }

}
