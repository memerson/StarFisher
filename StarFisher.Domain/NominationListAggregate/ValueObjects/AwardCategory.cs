﻿using System.Collections.Generic;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class AwardCategory : ValueObject<AwardCategory>
    {
        public static readonly AwardCategory QuarterlyAwards = new AwardCategory(@"Quarterly Awards");
        public static readonly AwardCategory SuperStarAwards = new AwardCategory(@"Super Star Awards");

        public static readonly AwardCategory Invalid = new AwardCategory(@"INVALID");

        private AwardCategory(string value)
        {
            Value = value;
        }

        public static IReadOnlyList<AwardCategory> ValidAwardsCategories { get; } = new List<AwardCategory>
        {
            QuarterlyAwards,
            SuperStarAwards
        };

        public string Value { get; }

        protected override bool EqualsCore(AwardCategory other)
        {
            return other.Value == Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}