﻿using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.DisqualifyNominees.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.DisqualifyNominees
{
    public class DisqualifyNomineesMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandText = @"Disqualify a nominee";

        public DisqualifyNomineesMenuItemCommand(IStarFisherContext context) : base(context, CommandText) { }

        public DisqualifyNomineesMenuItemCommand() : base(CommandText) { }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            do
            {
                if (!DisqualifyNominee())
                    return CommandOutput.None.Success;
            } while (GetDisqualifyAnotherNominee());

            return CommandOutput.None.Success;
        }

        private bool DisqualifyNominee()
        {
            var nominationList = Context.NominationListContext.NominationList;
            var parameter = new NomineeToDisqualifyParameter(nominationList.Nominees);
            if (!TryGetArgumentValue(parameter, out Person nomineeToDisqualify))
                return false;

            nominationList.DisqualifyNominee(nomineeToDisqualify);
            return true;
        }

        private static bool GetDisqualifyAnotherNominee()
        {
            return TryGetArgumentValue(new DisqualifyAnotherNomineeParameter(), out bool disqualifyAnother) && disqualifyAnother;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized && Context.NominationListContext.HasNominationListLoaded;
        }
    }
}
