using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.SelectAwardWinner.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.SelectAwardWinner
{
    public class SelectAwardWinnerMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Select a nominee as an award winner";

        public SelectAwardWinnerMenuItemCommand(IStarFisherContext context) : base(context, CommandTitle)
        {
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            do
            {
                if (!TryGetAwardType(out AwardType awardType))
                    break;

                SelectAwardWinner(awardType);

            } while (GetSelectAnotherAwardWinner());

            return CommandOutput.None.Success;
        }

        private static bool TryGetAwardType(out AwardType awardType)
        {
            var parameter = new AwardTypeParameter();
            return TryGetArgumentValue(parameter, out awardType);
        }

        private void SelectAwardWinner(AwardType awardType)
        {
            var nominationList = Context.NominationListContext.NominationList;
            var nominees = nominationList.GetNomineesForAward(awardType, true);
            var parameter = new NomineeToSelectAsAwardWinnerParameter(awardType, nominees);

            if (!TryGetArgumentValue(parameter, out Person nominee))
                return;

            nominationList.SelectNomineeAsAwardWinner(awardType, nominee);
        }

        private static bool GetSelectAnotherAwardWinner()
        {
            var parameter = new SelectAnotherAwardWinnerParameter();
            return TryGetArgumentValue(parameter, out bool selectAnotherAwardWinner) && selectAnotherAwardWinner;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized && Context.NominationListContext.HasNominationListLoaded;
        }
    }
}
