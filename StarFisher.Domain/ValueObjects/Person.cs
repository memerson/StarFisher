﻿using System;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class Person : ValueObject<Person>
    {
        private Person(PersonName name, OfficeLocation officeLocation, EmailAddress emailAddress)
        {
            Name = name;
            OfficeLocation = officeLocation;
            EmailAddress = emailAddress;
        }

        public static Person Create(PersonName name, OfficeLocation officeLocation, EmailAddress emailAddress)
        {
            return new Person(
                name ?? throw new ArgumentNullException(nameof(name)),
                officeLocation ?? throw new ArgumentNullException(nameof(officeLocation)),
                emailAddress ?? throw new ArgumentNullException(nameof(emailAddress)));
        }

        public PersonName Name { get; }

        public OfficeLocation OfficeLocation { get; }

        public EmailAddress EmailAddress { get; }

        protected override bool EqualsCore(Person other)
        {
            return Name == other.Name && OfficeLocation == other.OfficeLocation && EmailAddress == other.EmailAddress;
        }

        protected override int GetHashCodeCore()
        {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ OfficeLocation.GetHashCode();
            hashCode = (hashCode * 397) ^ EmailAddress.GetHashCode();
            return hashCode;
        }
    }
}
