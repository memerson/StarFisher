using System;
using System.Collections.Generic;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.SelectAwardWinner.Parameters
{
    public class NomineeToSelectAsAwardWinnerParameter : NomineeParameterBase
    {
        private readonly AwardType _awardType;

        public NomineeToSelectAsAwardWinnerParameter(AwardType awardType, IReadOnlyCollection<Person> nominees) :
            base(nominees)
        {
            _awardType = awardType ?? throw new ArgumentNullException(nameof(awardType));
            RegisterAbortInput(@"stop");
        }

        protected override string GetListItemLabel(Person listItem)
        {
            return $@"{listItem.Name.FullName} ({listItem.OfficeLocation.Name})";
        }

        protected override void WriteCallToAction()
        {
            WriteCallToAction(
                $@"Enter the number of the nominee you want to select as a {
                        _awardType.PrettyName
                    } winner, or enter 'stop' to stop selecting a {_awardType.PrettyName} winner.");
        }
    }
}