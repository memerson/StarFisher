using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class OfficeLocation : ValueObject<OfficeLocation>
    {
        public static readonly OfficeLocation Columbia = new OfficeLocation(@"Columbia MD (formerly Laurel)", @"Columbia");
        public static readonly OfficeLocation EchoBoulder = new OfficeLocation(@"Echo - Boulder", @"Boulder");
        public static readonly OfficeLocation EchoBrentwood = new OfficeLocation(@"Echo - Brentwood", @"Brentwood");
        public static readonly OfficeLocation EchoSanDiego = new OfficeLocation(@"Echo - San Diego", @"San Diego");
        public static readonly OfficeLocation Hccs = new OfficeLocation(@"HCCS - Jericho NY", @"Jericho");

        // TODO: Remove next quarter since this is no longer a HSTM office location.
        public static readonly OfficeLocation HighlandRidge = new OfficeLocation(@"Nashville - Highland Ridge (Marriott Dr.)", @"Highland Ridge");
        public static readonly OfficeLocation Morrisey = new OfficeLocation(@"Morrisey - Chicago", @"Chicago");
        public static readonly OfficeLocation NashvilleCorporate = new OfficeLocation(@"Nashville - Corporate (Downtown + Brentwood Sales)", @"Nashville Downtown");
        public static readonly OfficeLocation Remote = new OfficeLocation(@"Remote - Home Office and HEI", @"Remote");

        private static readonly List<OfficeLocation> ValidEmployeeOfficeLocations = new List<OfficeLocation>
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
        public static readonly OfficeLocation EiaTeamMember = new OfficeLocation(@"EIA Team Member", @"EIA Team Member");

        private OfficeLocation(string surveyName, string conciseName)
        {
            SurveyName = surveyName;
            ConciseName = conciseName;
        }

        public string SurveyName { get; }

        public string ConciseName { get; }

        public static IReadOnlyCollection<OfficeLocation> OfficeLocationsForCertificatePrinting =>
            new List<OfficeLocation>
            {
                NashvilleCorporate,
                Remote
            };

        internal static OfficeLocation Create(string officeLocationSurveyName)
        {
            return ValidEmployeeOfficeLocations.FirstOrDefault(ol => ol.SurveyName == officeLocationSurveyName) ?? Invalid;
        }

        protected override bool EqualsCore(OfficeLocation other)
        {
            return string.Equals(SurveyName, other.SurveyName);
        }

        protected override int GetHashCodeCore()
        {
            return SurveyName.GetHashCode();
        }

        public override string ToString()
        {
            return SurveyName;
        }

        public static implicit operator string(OfficeLocation officeLocation) => officeLocation?.ToString();
    }
}
