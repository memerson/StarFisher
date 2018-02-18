﻿using System;
using StarFisher.Domain.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Entities
{
    public abstract class AwardWinnerBase : Entity
    {
        protected AwardWinnerBase(int id, Person person, AwardType awardType, AwardAmount awardAmount)
            : base(id)
        {
            Person = person ?? throw new ArgumentNullException(nameof(person));
            AwardType = awardType ?? throw new ArgumentNullException(nameof(awardType));
            AwardAmount = awardAmount ?? throw new ArgumentNullException(nameof(awardAmount));
        }

        internal Person Person { get; private set; }

        public PersonName Name => Person.Name;

        public OfficeLocation OfficeLocation => Person.OfficeLocation;

        public EmailAddress EmailAddress => Person.EmailAddress;

        public AwardType AwardType { get; }

        public AwardAmount AwardAmount { get; }

        internal void UpdateWinnerName(PersonName newWinnerName)
        {
            Person = Person.UpdateName(newWinnerName);
        }

        internal void UpdateWinnerEmailAddress(EmailAddress newEmailAddress)
        {
            Person = Person.UpdateEmailAddress(newEmailAddress);
        }
    }
}
