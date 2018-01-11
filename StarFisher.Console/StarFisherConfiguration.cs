using System.Collections.Generic;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook;

namespace StarFisher.Console
{
    public class StarFisherConfiguration : IEmailConfiguration
    {
        public IReadOnlyCollection<EmailAddress> HumanResourcesEmailAddresses => new List<EmailAddress>
        {
            EmailAddress.Create("donna.snyder@healthstream.com"),
            EmailAddress.Create("tara.martin@healthstream.com")
        };

        public IReadOnlyCollection<PersonName> HumanResourcesPersonNames => new List<PersonName>
        {
            PersonName.Create("Donna Snyder"),
            PersonName.Create("Tara Marting")
        };

        public IReadOnlyCollection<EmailAddress> EiaChairPersonEmailAddresses => new List<EmailAddress>
        {
            EmailAddress.Create("ian.smith@healthstream.com")
        };

        public IReadOnlyCollection<PersonName> EiaChairPersonNames => new List<PersonName>
        {
            PersonName.Create("Ian Smith")
        };
    }
}
