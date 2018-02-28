using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.UnselectAwardWinner.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.UnselectAwardWinner
{
    public class UnselectAwardWinnerMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Unselect award winners";

        public UnselectAwardWinnerMenuItemCommand(IStarFisherContext context) : base(context, CommandTitle)
        {
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            do
            {
                if (!TryUnselectAwardWinner())
                    break;

            } while (GetUnelectAnotherAwardWinner());

            return CommandOutput.None.Success;
        }

        private bool TryUnselectAwardWinner()
        {
            var nominationList = Context.NominationListContext.NominationList;
            var awardWinners = nominationList.AwardWinners;
            var parameter = new AwardWinnerToUnselectParameter(awardWinners);

            if (!TryGetArgumentValue(parameter, out AwardWinner awardWinner))
                return false;

            nominationList.UnselectAwardWinner(awardWinner);
            return true;
        }

        private static bool GetUnelectAnotherAwardWinner()
        {
            var parameter = new UnselectAnotherAwardWinnerParameter();
            return TryGetArgumentValue(parameter, out bool selectAnotherAwardWinner) && selectAnotherAwardWinner;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized
                   && Context.NominationListContext.HasNominationListLoaded
                   && Context.NominationListContext.NominationList.HasAwardWinners;
        }
    }
}
