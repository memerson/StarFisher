using System;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNomineeWriteUps.Parameters;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeWriteUps
{
    public class FixNomineeWriteUpsMenuItemCommand : MenuItemCommandBase<FixNomineeWriteUpsMenuItemCommand.Input>
    {
        public FixNomineeWriteUpsMenuItemCommand() : base(@"Fix nomination write-ups") { }

        protected override CommandResult<CommandOutput.None> RunCore(Input input)
        {
            var nominationList = input.NominationList;

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
            var argument = parameter.GetArgument();

            while (argument.ArgumentType == ArgumentType.Invalid)
            {
                parameter.PrintInvalidArgumentMessage();
                argument = parameter.GetArgument();
            }

            return argument.Value;
        }

        private bool TryGetNewNominationWriteUp(Nomination nomination, out NominationWriteUp newWriteUp)
        {
            var parameter = new NewWriteUpParameter(nomination.NomineeName, nomination.WriteUp);
            var argument = parameter.GetArgument();

            while (argument.ArgumentType == ArgumentType.Invalid)
            {
                parameter.PrintInvalidArgumentMessage();
                argument = parameter.GetArgument();
            }

            if (argument.ArgumentType == ArgumentType.Abort ||
                argument.ArgumentType == ArgumentType.NoValue)
            {
                newWriteUp = null;
                return false;
            }

            newWriteUp = argument.Value;
            return true;
        }

        public class Input : CommandInput
        {
            public Input(NominationList nominationList)
            {
                NominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
            }

            public NominationList NominationList { get; }
        }
    }
}
