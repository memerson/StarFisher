using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.SelectAwardWinner.Parameters
{
    public class AwardTypeParameter : AwardTypeParameterBase
    {
        public AwardTypeParameter(AwardCategory awardCategory)
            : base(awardCategory)
        {
            RegisterAbortInput(@"stop");
        }

        protected override void WriteCallToAction()
        {
            WriteLine(
                @"Enter the number of the award for which you want to select a winner, or enter 'stop' to stop selecting award winners.");
        }
    }
}