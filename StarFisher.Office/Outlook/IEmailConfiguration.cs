using System;
using System.Collections.Generic;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Outlook
{
    public interface IEmailConfiguration
    {
        IReadOnlyCollection<EmailAddress> HumanResourcesEmailAddresses { get; }

        IReadOnlyCollection<PersonName> HumanResourcesPersonNames { get; }

        IReadOnlyCollection<EmailAddress> EiaChairPersonEmailAddresses { get; }

        IReadOnlyCollection<PersonName> EiaChairPersonNames { get; }
    }
}
