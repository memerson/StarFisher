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

        DirectoryPath StarAwardsDirectoryPath { get; }

        INominationListContext NominationListContext { get; }

        void Initialize(DirectoryPath starAwardsDirectoryPath, Year year, Quarter quarter, Person eiaChairPerson,
            ICollection<Person> hrPeople, ICollection<Person> luncheonPlannerPeople, Person certificatePrinterPerson);
    }

    public class StarFisherContext : IStarFisherContext
    {
        public static readonly StarFisherContext Instance = new StarFisherContext();
        private Person _certificatePrinterPerson;
        private Person _eiaChairPerson;
        private IReadOnlyList<Person> _hrPeople;
        private IReadOnlyList<Person> _luncheonPlannerPeople;
        private NominationListContext _nominationListContext;
        private Quarter _quarter;
        private DirectoryPath _starAwardsDirectoryPath;
        private WorkingDirectoryPath _workingDirectoryPath;
        private Year _year;

        private StarFisherContext()
        {
        }

        public void Initialize(DirectoryPath starAwardsDirectoryPath, Year year, Quarter quarter, Person eiaChairPerson,
            ICollection<Person> hrPeople, ICollection<Person> luncheonPlannerPeople, Person certificatePrinterPerson)
        {
            _starAwardsDirectoryPath = starAwardsDirectoryPath ??
                                    throw new ArgumentNullException(nameof(starAwardsDirectoryPath));
            _year = year ?? throw new ArgumentNullException(nameof(year));
            _quarter = quarter ?? throw new ArgumentNullException(nameof(quarter));
            _workingDirectoryPath = _starAwardsDirectoryPath.GetWorkingDirectory(year, quarter);
            _eiaChairPerson = eiaChairPerson ?? throw new ArgumentNullException(nameof(eiaChairPerson));
            _hrPeople = hrPeople?.ToList() ?? throw new ArgumentNullException(nameof(hrPeople));
            _luncheonPlannerPeople = luncheonPlannerPeople?.ToList() ??
                                     throw new ArgumentNullException(nameof(luncheonPlannerPeople));
            _certificatePrinterPerson = certificatePrinterPerson ??
                                        throw new ArgumentNullException(nameof(certificatePrinterPerson));

            var nominationListRepository = new NominationListRepository(_workingDirectoryPath);

            _nominationListContext = new NominationListContext(nominationListRepository);

            IsInitialized = true;
        }

        public bool IsInitialized { get; private set; }

        public DirectoryPath StarAwardsDirectoryPath
        {
            get
            {
                CheckIsInitialized();
                return _starAwardsDirectoryPath;
            }
        }

        public WorkingDirectoryPath WorkingDirectoryPath
        {
            get
            {
                CheckIsInitialized();
                return _workingDirectoryPath;
            }
        }

        public Year Year
        {
            get
            {
                CheckIsInitialized();
                return _year;
            }
        }

        public Quarter Quarter
        {
            get
            {
                CheckIsInitialized();
                return _quarter;
            }
        }

        public Person EiaChairPerson
        {
            get
            {
                CheckIsInitialized();
                return _eiaChairPerson;
            }
        }

        public IReadOnlyList<Person> HrPeople
        {
            get
            {
                CheckIsInitialized();
                return _hrPeople;
            }
        }

        public IReadOnlyList<Person> LuncheonPlannerPeople
        {
            get
            {
                CheckIsInitialized();
                return _luncheonPlannerPeople;
            }
        }

        public Person CertificatePrinterPerson
        {
            get
            {
                CheckIsInitialized();
                return _certificatePrinterPerson;
            }
        }

        public INominationListContext NominationListContext
        {
            get
            {
                CheckIsInitialized();
                return _nominationListContext;
            }
        }

        private void CheckIsInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("StarFisherContext not yet initialized");
        }
    }
}