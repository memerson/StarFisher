﻿using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Context
{
    public interface IStarFisherContext : IConfiguration
    {
        bool IsInitialized { get; }

        DirectoryPath StarAwardsDirectoryPath { get; }

        INominationListContext NominationListContext { get; }

        void Initialize(DirectoryPath starAwardsDirectoryPath, AwardsPeriod awardsPeriod, Person eiaCoChair1,
            Person eiaCoChair2, ICollection<Person> hrPeople, ICollection<Person> luncheonPlannerPeople,
            Person certificatePrinterPerson);
    }

    public class StarFisherContext : IStarFisherContext
    {
        public static readonly StarFisherContext Instance = new StarFisherContext();
        private AwardsPeriod _awardsPeriod;
        private Person _certificatePrinterPerson;
        private Person _eiaCoChair1;
        private Person _eiaCoChair2;
        private IReadOnlyList<Person> _hrPeople;
        private IReadOnlyList<Person> _luncheonPlannerPeople;
        private NominationListContext _nominationListContext;
        private DirectoryPath _starAwardsDirectoryPath;
        private WorkingDirectoryPath _workingDirectoryPath;

        private StarFisherContext()
        {
        }

        public void Initialize(DirectoryPath starAwardsDirectoryPath, AwardsPeriod awardsPeriod, Person eiaCoChair1,
            Person eiaCoChair2, ICollection<Person> hrPeople, ICollection<Person> luncheonPlannerPeople,
            Person certificatePrinterPerson)
        {
            _starAwardsDirectoryPath = starAwardsDirectoryPath ??
                                       throw new ArgumentNullException(nameof(starAwardsDirectoryPath));
            _awardsPeriod = awardsPeriod ?? throw new ArgumentNullException(nameof(awardsPeriod));
            _workingDirectoryPath = _starAwardsDirectoryPath.GetWorkingDirectory(_awardsPeriod);
            _eiaCoChair1 = eiaCoChair1 ?? throw new ArgumentNullException(nameof(eiaCoChair1));
            _eiaCoChair2 = eiaCoChair2 ?? throw new ArgumentNullException(nameof(eiaCoChair2));
            _hrPeople = hrPeople?.ToList() ?? throw new ArgumentNullException(nameof(hrPeople));
            _luncheonPlannerPeople = luncheonPlannerPeople?.ToList() ??
                                     throw new ArgumentNullException(nameof(luncheonPlannerPeople));
            _certificatePrinterPerson = certificatePrinterPerson ??
                                        throw new ArgumentNullException(nameof(certificatePrinterPerson));

            var nominationListRepository = new NominationListRepository(_workingDirectoryPath);

            _nominationListContext = new NominationListContext(nominationListRepository, awardsPeriod.AwardCategory);

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

        public AwardsPeriod AwardsPeriod
        {
            get
            {
                CheckIsInitialized();
                return _awardsPeriod;
            }
        }

        public Person EiaCoChair1
        {
            get
            {
                CheckIsInitialized();
                return _eiaCoChair1;
            }
        }

        public Person EiaCoChair2
        {
            get
            {
                CheckIsInitialized();
                return _eiaCoChair2;
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