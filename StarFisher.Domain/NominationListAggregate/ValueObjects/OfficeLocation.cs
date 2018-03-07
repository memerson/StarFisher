using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class OfficeLocation : ValueObject<OfficeLocation>
    {
        private readonly HashSet<string> _surveyNames;

        public static readonly OfficeLocation Columbia =
            new OfficeLocation(@"Columbia", @"Columbia MD (formerly Laurel)", @"Columbia MD (fmr. Laurel)");

        public static readonly OfficeLocation EchoBoulder = new OfficeLocation(@"Boulder", @"Echo - Boulder");
        public static readonly OfficeLocation EchoBrentwood = new OfficeLocation(@"Brentwood", @"Echo - Brentwood");
        public static readonly OfficeLocation EchoSanDiego = new OfficeLocation(@"San Diego", @"Echo - San Diego", @"Echo - San Diego CA");
        public static readonly OfficeLocation Hccs = new OfficeLocation(@"Jericho", @"HCCS - Jericho NY");

        // TODO: Remove next quarter since this is no longer a HSTM office location.
        public static readonly OfficeLocation HighlandRidge =
            new OfficeLocation(@"Highland Ridge", @"Nashville - Highland Ridge (Marriott Dr.)");

        public static readonly OfficeLocation Morrisey = new OfficeLocation(@"Chicago", @"Morrisey - Chicago", @"Morrisey - Chicago IL");

        public static readonly OfficeLocation NashvilleCorporate =
            new OfficeLocation(@"Nashville Downtown", @"Nashville - Corporate (Downtown + Brentwood Sales)");

        public static readonly OfficeLocation Remote = new OfficeLocation(@"Remote", @"Remote - Home Office and HEI");

        public static readonly IReadOnlyList<OfficeLocation> ValidEmployeeOfficeLocations = new List<OfficeLocation>
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

        public static readonly OfficeLocation Invalid = new OfficeLocation(@"INVALID", @"INVALID");

        public static readonly OfficeLocation EiaTeamMember =
            new OfficeLocation(@"EIA Team Member", @"EIA Team Member");

        private OfficeLocation(string name, params string[] surveyNames)
        {
            Name = name;
            _surveyNames = new HashSet<string>(surveyNames ?? Enumerable.Empty<string>());
        }

        public string Name { get; }

        public static IReadOnlyList<OfficeLocation> OfficeLocationsForCertificatePrinting =>
            new List<OfficeLocation>
            {
                NashvilleCorporate,
                Remote
            };

        internal static OfficeLocation FindByName(string officeLocationSurveyName)
        {
            return ValidEmployeeOfficeLocations.FirstOrDefault(ol => 
                ol.Name == officeLocationSurveyName ||
                ol._surveyNames.Contains(officeLocationSurveyName)) ??
                   Invalid;
        }

        protected override bool EqualsCore(OfficeLocation other)
        {
            return string.Equals(Name, other.Name);
        }

        protected override int GetHashCodeCore()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator string(OfficeLocation officeLocation)
        {
            return officeLocation?.ToString();
        }
    }
}