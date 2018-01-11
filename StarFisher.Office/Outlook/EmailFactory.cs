using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Outlook
{
    public interface IEmailFactory
    {
        IEmail GetHumanResourcesNomineeValidationEmail(NominationList nominationList);
    }

    public class EmailFactory : IEmailFactory
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public EmailFactory(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public IEmail GetHumanResourcesNomineeValidationEmail(NominationList nominationList)
        {
            // TODO: null arg check
            return new HumanResourcesNomineeValidationEmail(_emailConfiguration, nominationList);
        }
    }
}
