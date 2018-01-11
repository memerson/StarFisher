using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class OfficeLocation : ValueObject<OfficeLocation>
    {
        public static readonly OfficeLocation Columbia = new OfficeLocation(@"Columbia MD (formerly Laurel)");
        public static readonly OfficeLocation EchoBoulder = new OfficeLocation(@"Echo - Boulder");
        public static readonly OfficeLocation EchoBrentwood = new OfficeLocation(@"Echo - Brentwood");
        public static readonly OfficeLocation EchoSanDiego = new OfficeLocation(@"Echo - San Diego");
        public static readonly OfficeLocation Hccs = new OfficeLocation(@"HCCS - Jericho NY");
        public static readonly OfficeLocation HighlandRidge = new OfficeLocation(@"Nashville - Highland Ridge (Marriott Dr.)");
        public static readonly OfficeLocation Morrisey = new OfficeLocation(@"Morrisey - Chicago");
        public static readonly OfficeLocation NashvilleCorporate = new OfficeLocation(@"Nashville - Corporate (Downtown + Brentwood Sales)");
        public static readonly OfficeLocation Remote = new OfficeLocation(@"Remote - Home Office and HEI");

        private static readonly List<OfficeLocation> ValidOfficeLocations = new List<OfficeLocation>
        {
            Columbia,
            EchoBoulder,
            EchoBrentwood,
            EchoSanDiego,
            Hccs,
            HighlandRidge,
            Morrisey,
            NashvilleCorporate,
            Remote
        };

        public static readonly OfficeLocation Invalid = new OfficeLocation(@"INVALID");

        private OfficeLocation(string value)
        {
            Value = value;
        }

        public string Value { get; }

        internal static OfficeLocation Create(string officeLocationText)
        {
            var officeLocation = new OfficeLocation(officeLocationText);
            return ValidOfficeLocations.All(ol => ol != officeLocation) ? Invalid : officeLocation;
        }

        protected override bool EqualsCore(OfficeLocation other)
        {
            return string.Equals(Value, other.Value);
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
