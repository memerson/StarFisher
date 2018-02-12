using System.Collections.Generic;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;

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
            var nomineeName = listItem.NomineeName.FullName;
            var officeLocation = listItem.NomineeOfficeLocation.ConciseName;
            var nominatorName = listItem.NominatorName.FullName;

            return $@"For {nomineeName} ({officeLocation}) nominated by {nominatorName}";
        }

        protected override string GetSelectionInstructions()
        {
            return @"Enter the number of the nomination you want to remove, or enter 'stop' to stop removing nominations.";
        }
    }
}
