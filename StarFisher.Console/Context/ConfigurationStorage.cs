﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Context
{
    public interface IConfigurationStorage
    {
        void SaveConfiguration();
    }

    public class ConfigurationStorage : IConfigurationStorage
    {
        public void SaveConfiguration()
        {
            if (!StarFisherContext.Current.IsInitialized)
                return;

            var filePath = GetStarFisherConfigurationFilePath(true);

            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, new Configuration(StarFisherContext.Current));
            }
        }

        public bool InitializeFromConfiguration(out Exception exception)
        {
            exception = null;
            var filePath = GetStarFisherConfigurationFilePath();

            if (!File.Exists(filePath))
                return false;

            try
            {
                using (var file = File.OpenText(filePath))
                {
                    var serializer = new JsonSerializer();
                    var configuration = (Configuration) serializer.Deserialize(file, typeof(Configuration));
                    configuration.InitializeContext();
                    return true;
                }
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        private static string GetStarFisherAppDataDirectoryPath(bool create = false)
        {
            var appDataDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var starFisherAppDataDirectoryPath = Path.Combine(appDataDirectoryPath, @"StarFisher");

            if(create)
                Directory.CreateDirectory(starFisherAppDataDirectoryPath);

            return starFisherAppDataDirectoryPath;
        }

        private static string GetStarFisherConfigurationFilePath(bool createDirectory = false)
        {
            return Path.Combine(GetStarFisherAppDataDirectoryPath(createDirectory), @"configuration.json");
        }

        private class Configuration
        {
            public Configuration() { }

            public Configuration(IStarFisherContext context) : this()
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                WorkingDirectoryPath = context.WorkingDirectoryPath.Value;
                Year = context.Year.Value;
                Quarter = context.Quarter.NumericValue;
                EiaChairPerson = Convert(context.EiaChairPerson);
                HrPeople = Convert(context.HrPeople);
                LuncheonPlannerPeople = Convert(context.LuncheonPlannerPeople);
                CertificatePrinterPerson = Convert(context.CertificatePrinterPerson);
            }

            public void InitializeContext()
            {
                StarFisherContext.Current.Initialize(
                    DirectoryPath.Create(WorkingDirectoryPath),
                    Domain.ValueObjects.Year.Create(Year),
                    Domain.ValueObjects.Quarter.Create(Quarter),
                    Convert(EiaChairPerson),
                    Convert(HrPeople),
                    Convert(LuncheonPlannerPeople),
                    Convert(CertificatePrinterPerson));
            }

            private static Person Convert(Domain.ValueObjects.Person person)
            {
                return new Person {Name = person.Name.FullName, EmailAddress = person.EmailAddress.Value};
            }

            private static Domain.ValueObjects.Person Convert(Person person)
            {
                return Domain.ValueObjects.Person.Create(PersonName.Create(person.Name), OfficeLocation.EiaTeamMember,
                    EmailAddress.Create(person.EmailAddress));
            }

            private static List<Person> Convert(IEnumerable<Domain.ValueObjects.Person> people)
            {
                return people
                    .Select(Convert)
                    .ToList();
            }

            private static List<Domain.ValueObjects.Person> Convert(IEnumerable<Person> people)
            {
                return people.Select(Convert).ToList();
            }

            public string WorkingDirectoryPath { get; set; }

            public int Year { get; set; }

            public int Quarter { get; set; }

            public Person EiaChairPerson { get; set; }

            public List<Person> HrPeople { get; set; }

            public List<Person> LuncheonPlannerPeople { get; set; }

            public Person CertificatePrinterPerson { get; set; }

            public class Person
            {
                public string Name { get; set; }

                public string EmailAddress { get; set; }
            }
        }
    }
}