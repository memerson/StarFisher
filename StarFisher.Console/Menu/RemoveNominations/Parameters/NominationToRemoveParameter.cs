using System.Collections.Generic;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.NominationListAggregate.Entities;

namespace StarFisher.Console.Menu.RemoveNominations.Parameters
{
    public class NominationToRemoveParameter : ListItemSelectionParameterBase<Nomination>
    {
        public NominationToRemoveParameter(IReadOnlyList<Nomination> nominations)
            : base(nominations, @"nominations")
        {
            RegisterAbortInput(@"stop");
        }

        protected override string GetListItemLabel(Nomination listItem)
        {
            var awardType = listItem.AwardType;
            var nomineeName = listItem.NomineeName.FullName;
            var officeLocation = listItem.NomineeOfficeLocation.Name;
            var nominatorName = listItem.NominatorName.FullName;

            return $@"{awardType.PrettyName} nomination for {nomineeName} ({officeLocation}) nominated by {nominatorName}";
        }

        protected override void WriteCallToAction()
        {
            WriteCallToAction(
                @"Enter the number of the nomination you want to remove, or enter 'stop' to stop removing nominations.");
        }
    }
}