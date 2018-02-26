using System;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;

namespace StarFisher.Console.Menu.RemoveNominations.Parameters
{
    public class RemoveOnlyNominationParameter : YesOrNoParameterBase
    {
        private readonly Nomination _nomination;

        public RemoveOnlyNominationParameter(Nomination nomination)
        {
            _nomination = nomination ?? throw new ArgumentNullException(nameof(nomination));
        }

        protected override string GetCallToActionText()
        {
            var awardName = _nomination.AwardType.PrettyName;
            var nomineeName = _nomination.NomineeName.FullName;

            return $@"This is the only {awardName} nomination for {nomineeName}. Removing it essentially disqualifies them for that award. Do you want to remove this nomination (yes or no)?";
        }
    }
}