using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.UnselectAwardWinner.Parameters
{
    public class AwardWinnerToUnselectParameter : ListItemSelectionParameterBase<AwardWinner>
    {
        public AwardWinnerToUnselectParameter(IReadOnlyList<AwardWinner> awardWinners) 
            : base(awardWinners?.OrderBy(w => w.Name.FullName).ToList(), @"award winners")
        {
            RegisterAbortInput(@"stop");
        }

        protected override string GetListItemLabel(AwardWinner listItem)
        {
            return $@"{listItem.Name} from {listItem.OfficeLocation.ConciseName}, {listItem.AwardType.PrettyName} winner";
        }

        protected override void WriteCallToAction()
        {
            WriteCallToAction($@"Enter the number of the award winner you want to unselect, or enter 'stop' to stop unselecting award winners.");
        }
    }
}
