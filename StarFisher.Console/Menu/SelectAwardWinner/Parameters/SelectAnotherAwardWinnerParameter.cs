using StarFisher.Console.Menu.Common.Parameters;

namespace StarFisher.Console.Menu.SelectAwardWinner.Parameters
{
    public class SelectAnotherAwardWinnerParameter : YesOrNoParameterBase
    {
        protected override string GetCallToActionText()
        {
            return @"Would you like to select another award winner (yes or no)?";
        }
    }
}