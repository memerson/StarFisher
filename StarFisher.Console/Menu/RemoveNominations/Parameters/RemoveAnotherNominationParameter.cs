using StarFisher.Console.Menu.Common.Parameters;

namespace StarFisher.Console.Menu.RemoveNominations.Parameters
{
    public class RemoveAnotherNominationParameter : YesOrNoParameterBase
    {
        protected override string GetCallToActionText()
        {
            return @"Would you like to remove another nomination (yes or no)?";
        }
    }
}
