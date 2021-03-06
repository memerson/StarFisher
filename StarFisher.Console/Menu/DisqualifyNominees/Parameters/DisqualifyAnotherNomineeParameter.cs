﻿using StarFisher.Console.Menu.Common.Parameters;

namespace StarFisher.Console.Menu.DisqualifyNominees.Parameters
{
    public class DisqualifyAnotherNomineeParameter : YesOrNoParameterBase
    {
        protected override string GetCallToActionText()
        {
            return @"Would you like to disqualify any other nominees (yes or no)?";
        }
    }
}