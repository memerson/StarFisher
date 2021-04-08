using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class OfficeLocation : ValueObject<OfficeLocation>
    {
        public static readonly OfficeLocation Boulder = new OfficeLocation(@"Boulder", @"VerityStream - Boulder", @"VerityStream - Boulder CO");

        public static readonly OfficeLocation SanDiego =
            new OfficeLocation(@"San Diego", @"VerityStream - San Diego", @"VerityStream - San Diego CA");

        public static readonly OfficeLocation Chicago =
            new OfficeLocation(@"Chicago", @"VerityStream - Chicago", @"VerityStream - Chicago IL");

        public static readonly OfficeLocation NashvilleCorporate =
            new OfficeLocation(@"Nashville Downtown", @"Nashville - Capitol View");

        public static readonly OfficeLocation HighlandRidge =
            new OfficeLocation(@"Highland Ridge", @"Nashville - Highland Ridge (Training Center only)");

        public static readonly OfficeLocation Jericho = new OfficeLocation(@"Jericho", @"HCCS - Jericho NY");

        public static readonly OfficeLocation Providigm = new OfficeLocation(@"Providigm", @"Providigm - Denver CO");

        public static readonly OfficeLocation Remote = new OfficeLocation(@"Remote", @"Remote - Home Office");

        public static readonly OfficeLocation Savannah = new OfficeLocation(@"Savannah", @"VerityStream - Savannah GA");

        public static readonly OfficeLocation Portland = new OfficeLocation(@"Portland", @"NurseGrid - Portland OR");

        public static readonly OfficeLocation Raleigh = new OfficeLocation(@"Raleigh", @"ShiftWizard - Raleigh NC");

        public static readonly OfficeLocation Ansos = new OfficeLocation(@"ANSOS", @"ANSOS - Nashville TN");

        public static readonly OfficeLocation ComplyALIGN = new OfficeLocation(@"ComplyALIGN", @"ComplyALIGN - Chicago IL");

        public static readonly OfficeLocation MyClinicalExchange = new OfficeLocation(@"myClinicalExchange", @"myClinicalExchange - Denver CO");

        public static readonly IReadOnlyList<OfficeLocation> ValidEmployeeOfficeLocations = new List<OfficeLocation>
        {
            Boulder,
            SanDiego,
            Chicago,
            NashvilleCorporate,
            HighlandRidge,
            Jericho,
            Providigm,
            Remote,
            Savannah,
            Portland,
            Raleigh,
            Ansos,
            ComplyALIGN,
            MyClinicalExchange
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
                HighlandRidge
            };

        public static IReadOnlyList<OfficeLocation> OfficeLocationsForCertificatePrinting =>
            ValidEmployeeOfficeLocations;

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