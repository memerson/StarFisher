﻿using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class Quarter : ValueObject<Quarter>
    {
        public static readonly Quarter First = new Quarter(1);
        public static readonly Quarter Second = new Quarter(2);
        public static readonly Quarter Third = new Quarter(3);
        public static readonly Quarter Fourth = new Quarter(4);

        private static readonly List<Quarter> ValidQuarters = new List<Quarter>
        {
            First,
            Second,
            Third,
            Fourth
        };

        public static readonly Quarter Invalid = new Quarter(0);

        private Quarter(int numericValue)
        {
            NumericValue = numericValue;
        }

        public static Quarter Create(int numericValue)
        {
            var quarter = ValidQuarters.FirstOrDefault(q => q.NumericValue == numericValue);
            return quarter ?? Invalid;
        }

        public static bool IsValid(int numericValue)
        {
            return ValidQuarters.Any(q => q.NumericValue == numericValue);
        }

        public int NumericValue { get; }

        public string Abbreviation => "Q" + NumericValue;

        public string FullName => "Quarter " + NumericValue;

        protected override bool EqualsCore(Quarter other)
        {
            return NumericValue == other.NumericValue;
        }

        protected override int GetHashCodeCore()
        {
            return NumericValue.GetHashCode();
        }

        public override string ToString()
        {
            return Abbreviation;
        }
    }
}
