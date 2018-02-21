using StarFisher.Console.Menu.Common.Parameters;

namespace StarFisher.Console.Menu.UnselectAwardWinner.Parameters
{
    public class UnselectAnotherAwardWinnerParameter : YesOrNoParameterBase
    {
        protected override string GetCallToActionText()
        {
            return @"Would you like to unselect another award winner (yes or no)?";
        }
    }
}
