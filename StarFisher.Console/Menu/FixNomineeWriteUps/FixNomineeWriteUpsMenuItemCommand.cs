using System.Linq;
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

        public FixNomineeWriteUpsMenuItemCommand() : base(CommandTitle)
        {
        }

        public FixNomineeWriteUpsMenuItemCommand(IStarFisherContext context) : base(context, CommandTitle)
        {
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized && Context.NominationListContext.HasNominationListLoaded;
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;

            if (nominationList.Nominations.Any(nomination => !ReviewNomination(nomination, nominationList)))
                return CommandOutput.None.Success;

            return CommandOutput.None.Success;
        }

        private bool ReviewNomination(Nomination nomination, NominationList nominationList)
        {
            if (!GetNominationWriteUpAction(nomination, out WriteUpActionParameter.Action action))
                return false;

            if (action == WriteUpActionParameter.Action.Continue)
                return true;

            var changedWriteUp = TryGetNewNominationWriteUp(nomination, out NominationWriteUp newWriteUp);

            if (changedWriteUp)
                nominationList.UpdateNominationWriteUp(nomination.Id, newWriteUp);

            return true;
        }

        private static bool GetNominationWriteUpAction(Nomination nomination, out WriteUpActionParameter.Action action)
        {
            var parameter = new WriteUpActionParameter(nomination.NomineeName, nomination.WriteUp);
            return TryGetArgumentValue(parameter, out action);
        }

        private static bool TryGetNewNominationWriteUp(Nomination nomination, out NominationWriteUp newWriteUp)
        {
            var parameter = new NewWriteUpParameter(nomination.NomineeName, nomination.WriteUp);
            return TryGetArgumentValue(parameter, out newWriteUp);
        }
    }
}