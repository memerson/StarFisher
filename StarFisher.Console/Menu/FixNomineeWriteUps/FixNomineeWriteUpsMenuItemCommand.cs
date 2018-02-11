using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNomineeWriteUps.Parameters;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeWriteUps
{
    public class FixNomineeWriteUpsMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Fix nomination write-ups";

        public FixNomineeWriteUpsMenuItemCommand() : base(CommandTitle) { }

        public FixNomineeWriteUpsMenuItemCommand(IStarFisherContext context) : base(context, CommandTitle) { }

        public override bool GetCanRun()
        {
            return Context.IsInitialized && Context.NominationListContext.HasNominationListLoaded;
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;

            foreach (var nomination in nominationList.Nominations)
            {
                var quitRequested = ReviewNomination(nomination, nominationList);
                if (quitRequested)
                    break;
            }

            return CommandOutput.None.Success;
        }

        private bool ReviewNomination(Nomination nomination, NominationList nominationList)
        {
            var action = GetNominationWriteUpAction(nomination);

            if (action == WriteUpActionParameter.Action.Stop)
                return true;

            if (action != WriteUpActionParameter.Action.Edit)
                return false;

            var changedWriteUp = TryGetNewNominationWriteUp(nomination, out NominationWriteUp newWriteUp);

            if (!changedWriteUp)
                return false;

            nominationList.UpdateNominationWriteUp(nomination.Id, newWriteUp);
            return false;
        }

        private WriteUpActionParameter.Action GetNominationWriteUpAction(Nomination nomination)
        {
            var parameter = new WriteUpActionParameter(nomination.NomineeName, nomination.WriteUp);
            var argument = parameter.GetValidArgument();
            return argument.Value;
        }

        private bool TryGetNewNominationWriteUp(Nomination nomination, out NominationWriteUp newWriteUp)
        {
            var parameter = new NewWriteUpParameter(nomination.NomineeName, nomination.WriteUp);
            var argument = parameter.GetValidArgument();

            if (argument.ArgumentType == ArgumentType.Abort)
            {
                newWriteUp = null;
                return false;
            }

            newWriteUp = argument.Value;
            return true;
        }
    }
}
