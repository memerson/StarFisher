using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Context
{
    public interface IStarFisherContext : IConfiguration
    {
        bool IsInitialized { get; }

        void Initialize(DirectoryPath workingDirectoryPath, Year year, Quarter quarter, Person eiaChairPerson,
            ICollection<Person> hrPeople, ICollection<Person> luncheonPlannerPeople, Person certificatePrinterPerson);

        INominationListContext NominationListContext { get; }
    }

    public class StarFisherContext : IStarFisherContext
    {
        private static readonly StarFisherContext SingletonContext = new StarFisherContext();

        private DirectoryPath _workingDirectoryPath;
        private Year _year;
        private Quarter _quarter;
        private Person _eiaChairPerson;
        private IReadOnlyList<Person> _hrPeople;
        private IReadOnlyList<Person> _luncheonPlannerPeople;
        private Person _certificatePrinterPerson;

        private StarFisherContext() { }

        public static StarFisherContext Current => SingletonContext;

        public void Initialize(DirectoryPath workingDirectoryPath, Year year, Quarter quarter, Person eiaChairPerson,
            ICollection<Person> hrPeople, ICollection<Person> luncheonPlannerPeople, Person certificatePrinterPerson)
        {
            WorkingDirectoryPath = workingDirectoryPath ??
                                   throw new ArgumentNullException(nameof(workingDirectoryPath));
            Year = year ?? throw new ArgumentNullException(nameof(year));
            Quarter = quarter ?? throw new ArgumentNullException(nameof(quarter));
            EiaChairPerson = eiaChairPerson ?? throw new ArgumentNullException(nameof(eiaChairPerson));
            HrPeople = hrPeople?.ToList() ?? throw new ArgumentNullException(nameof(hrPeople));
            LuncheonPlannerPeople = luncheonPlannerPeople?.ToList() ??
                                    throw new ArgumentNullException(nameof(luncheonPlannerPeople));
            CertificatePrinterPerson = certificatePrinterPerson ??
                                       throw new ArgumentNullException(nameof(certificatePrinterPerson));

            var nominationListRepository = new NominationListRepository(workingDirectoryPath);

            Context.NominationListContext.Initialize(nominationListRepository, year, quarter);

            IsInitialized = true;
        }

        public bool IsInitialized { get; private set; }

        public DirectoryPath WorkingDirectoryPath
        {
            get
            {
                CheckIsInitialized();
                return _workingDirectoryPath;
            }

            private set => _workingDirectoryPath = value;
        }

        public Year Year
        {
            get
            {
                CheckIsInitialized();
                return _year;
            }

            private set => _year = value;
        }

        public Quarter Quarter
        {
            get
            {
                CheckIsInitialized();
                return _quarter;
            }
            private set => _quarter = value;
        }

        public Person EiaChairPerson
        {
            get
            {
                CheckIsInitialized();
                return _eiaChairPerson;
            }
            private set => _eiaChairPerson = value;
        }

        public IReadOnlyList<Person> HrPeople
        {
            get
            {
                CheckIsInitialized();
                return _hrPeople;
            }
            private set => _hrPeople = value;
        }

        public IReadOnlyList<Person> LuncheonPlannerPeople
        {
            get
            {
                CheckIsInitialized();
                return _luncheonPlannerPeople;
            }
            private set => _luncheonPlannerPeople = value;
        }

        public Person CertificatePrinterPerson
        {
            get
            {
                CheckIsInitialized();
                return _certificatePrinterPerson;
            }
            private set => _certificatePrinterPerson = value;
        }

        public INominationListContext NominationListContext => Context.NominationListContext.Current;

        private void CheckIsInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("StarFisherContext not yet initialized");
        }
    }
}
