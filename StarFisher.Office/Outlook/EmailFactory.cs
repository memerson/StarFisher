using System;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Office.Word;

namespace StarFisher.Office.Outlook
{
    public interface IEmailFactory
    {
        IEmail GetHumanResourcesNomineeValidationEmail(NominationList nominationList);
        IEmail GetVotingSurveyReviewEmail(NominationList nominationList, string votingSurveyWebLink);
    }

    public class EmailFactory : IEmailFactory
    {
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly IMailMergeFactory _mailMergeFactory;

        public EmailFactory(IEmailConfiguration emailConfiguration, IMailMergeFactory mailMergeFactory)
        {
            _emailConfiguration = emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration));
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
    }
}
