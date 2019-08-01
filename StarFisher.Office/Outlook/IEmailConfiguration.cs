using System.Collections.Generic;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Office.Outlook
{
    public interface IEmailConfiguration
    {
        Person EiaCoChair1 { get; }

        Person EiaCoChair2 { get; }

        IReadOnlyList<Person> HrPeople { get; }

        IReadOnlyList<Person> LuncheonPlannerPeople { get; }

        Person CertificatePrinterPerson { get; }
    }
}