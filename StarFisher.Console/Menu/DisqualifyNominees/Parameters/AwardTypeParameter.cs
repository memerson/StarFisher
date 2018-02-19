using System.Collections.Generic;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.DisqualifyNominees.Parameters
{
    public class AwardTypeParameter : ListItemSelectionParameterBase<AwardType>
    {
        private static readonly IReadOnlyList<AwardType> AwardTypes = new List<AwardType>
        {
            AwardType.RisingStar,
            AwardType.StarValues
        };

        public AwardTypeParameter() : base(AwardTypes, @"awards")
        {
            RegisterAbortInput(@"stop");
        }

        protected override string GetListItemLabel(AwardType listItem)
        {
            return listItem.PrettyName;
        }

        protected override void WriteCallToAction()
        {
            WriteLine(
                @"Enter the number of the award for which you want to disqualify a nominee, or enter 'stop' to stop disqualifying nominees.");
        }
    }
}
