using System.Collections.Generic;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Office.Outlook
{
    public interface IEmailConfiguration
    {
        Person EiaChairPerson { get; }

        IReadOnlyList<Person> HrPeople { get; }

        IReadOnlyList<Person> LuncheonPlannerPeople { get; }

        Person CertificatePrinterPerson { get; }
    }
}