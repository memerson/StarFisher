using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class OfficeLocation : ValueObject<OfficeLocation>
    {
        public static readonly OfficeLocation Boulder = new OfficeLocation(@"Boulder", @"Verity - Boulder CO");
        public static readonly OfficeLocation Brentwood = new OfficeLocation(@"Brentwood", @"Verity - Brentwood TN");

        public static readonly OfficeLocation SanDiego =
            new OfficeLocation(@"San Diego", @"Verity - San Diego", @"Verity - San Diego CA");

        public static readonly OfficeLocation Jericho = new OfficeLocation(@"Jericho", @"HCCS - Jericho NY");

        // TODO: Remove next quarter since this is no longer a HSTM office location.
        public static readonly OfficeLocation HighlandRidge =
            new OfficeLocation(@"Highland Ridge", @"Nashville - Highland Ridge (Training Center only)");

        public static readonly OfficeLocation Chicago =
            new OfficeLocation(@"Chicago", @"Verity - Chicago", @"Verity - Chicago IL");

        public static readonly OfficeLocation NashvilleCorporate =
            new OfficeLocation(@"Nashville Downtown", @"Nashville - Corporate (Downtown + Brentwood Sales)");

        public static readonly OfficeLocation Remote = new OfficeLocation(@"Remote", @"Remote - Home Office");

        public static readonly IReadOnlyList<OfficeLocation> ValidEmployeeOfficeLocations = new List<OfficeLocation>
        {
            Boulder,
            Brentwood,
            SanDiego,
            Jericho,
            HighlandRidge,
            Chicago,
            NashvilleCorporate,
            Remote
        };

        public static readonly OfficeLocation Invalid = new OfficeLocation(@"INVALID", @"INVALID");

        public static readonly OfficeLocation EiaTeamMember =
            new OfficeLocation(@"EIA Team Member", @"EIA Team Member");

        private readonly HashSet<string> _surveyNames;

        private OfficeLocation(string name, params string[] surveyNames)
        {
            Name = name;
            _surveyNames = new HashSet<string>(surveyNames ?? Enumerable.Empty<string>());
        }

        public string Name { get; }

        public static IReadOnlyList<OfficeLocation> OfficeLocationsForLuncheon =>
            new List<OfficeLocation>
            {
                NashvilleCorporate,
                HighlandRidge,
                Brentwood
            };

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