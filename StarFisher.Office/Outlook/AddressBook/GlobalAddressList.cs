﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Outlook;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Utilities;
using OutlookApplication = Microsoft.Office.Interop.Outlook.Application;

namespace StarFisher.Office.Outlook.AddressBook
{
    public interface IGlobalAddressList
    {
        bool GetPersonExists(PersonName name);
        bool GetPersonExists(EmailAddress emailAddress);
    }

    public class GlobalAddressList : IGlobalAddressList
    {
        private static readonly Dictionary<string, List<Person>> PeopleByLastName = new Dictionary<string, List<Person>>(3000);
        private static readonly Dictionary<string, List<Person>> PeopleByEmailAddress = new Dictionary<string, List<Person>>(3000);

        static GlobalAddressList()
        {
            using (var com = new ComObjectManager())
            {
                var outlook = com.Get(() => new OutlookApplication());
                var session = com.Get(() => outlook.Session);
                var globalAddressList = com.Get(() => session.GetGlobalAddressList());
                var addressEntries = com.Get(() => globalAddressList.AddressEntries);

                var people = new List<Person>(3000);
                people.AddRange(addressEntries
                    .Cast<AddressEntry>()
                    .Select(ae => com.Get(() => ae))
                    .Select(ae => GetPerson(com, ae))
                    .Where(p => p != null));

                foreach(var group in people.GroupBy(p => p.LastName))
                    PeopleByLastName.Add(group.Key, group.ToList());

                foreach (var group in people.GroupBy(p => p.EmailAddress))
                    PeopleByEmailAddress.Add(group.Key, group.ToList());
            }
        }

        public bool GetPersonExists(PersonName name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return PeopleByLastName.TryGetValue(name.LastName, out List<Person> lastNameMatches) && 
                lastNameMatches.Any(p => p.FirstName.StartsWith(name.FirstName));
        }

        public bool GetPersonExists(EmailAddress emailAddress)
        {
            if (emailAddress == null)
                throw new ArgumentNullException(nameof(emailAddress));

            return PeopleByEmailAddress.ContainsKey(emailAddress.Value.ToLower());
        }

        //public PersonQueryResult QueryNominee(PersonName personName, EmailAddress emailAddress)
        //{
        //    if(personName == null)
        //        throw new ArgumentNullException(nameof(personName));
        //    if (emailAddress == null)
        //        throw new ArgumentNullException(nameof(emailAddress));

        //    if(!PeopleByLastName.TryGetValue(personName.LastName, out List<Person> lastNameMatches))
        //        return PersonQueryResult.NameNotFound;

        //    var fullNameMatches = lastNameMatches
        //        .Where(p => p.FirstName.StartsWith(personName.FirstName))
        //        .ToList();

        //    if(fullNameMatches.Count == 0)
        //        return PersonQueryResult.NameNotFound;

        //    if(fullNameMatches.Count > 1)
        //        return PersonQueryResult.MultipleNameMatchesFound;

        //    var fullNameMatch = fullNameMatches.First();

        //    if (string.Equals(fullNameMatch.EmailAddress, emailAddress.Value, StringComparison.InvariantCultureIgnoreCase))
        //        return PersonQueryResult.NameAndEmailAddressFound;

        //    return PersonQueryResult.EmailAddressDoesNotMatch;
        //}

        private static Person GetPerson(ComObjectManager com, AddressEntry addressEntry)
        {
            if (addressEntry.AddressEntryUserType != OlAddressEntryUserType.olExchangeUserAddressEntry &&
                addressEntry.AddressEntryUserType != OlAddressEntryUserType.olExchangeRemoteUserAddressEntry)
            {
                return null;
            }

            var exchangeUser = com.Get(addressEntry.GetExchangeUser);

            if (exchangeUser == null)
                return null;

            var person = new Person(exchangeUser);
            return person;
        }

        private class Person
        {
            public Person(ExchangeUser exchangeUser)
            {
                if (exchangeUser == null)
                    throw new ArgumentNullException(nameof(exchangeUser));

                FirstName = exchangeUser.FirstName ?? string.Empty;
                LastName = exchangeUser.LastName ?? string.Empty;
                EmailAddress = exchangeUser.PrimarySmtpAddress?.ToLower() ?? string.Empty;
            }

            public string FirstName { get; }

            public string LastName { get; }

            public string EmailAddress { get; }
        }
    }
}