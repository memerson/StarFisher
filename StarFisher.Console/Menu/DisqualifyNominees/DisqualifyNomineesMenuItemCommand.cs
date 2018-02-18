using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.DisqualifyNominees.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.DisqualifyNominees
{
    public class DisqualifyNomineesMenuItemCommand : MenuItemCommandBase
    {// TODO: Make award-specific
        private const string CommandTitle = @"Disqualify a nominee";

        public DisqualifyNomineesMenuItemCommand(IStarFisherContext context)
            : base(context, CommandTitle) { }

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

            if (Context.AwardWinnerListContext.HasAwardWinnerListLoaded)
            {
                var awardWinnerList = Context.AwardWinnerListContext.AwardWinnerList;
                awardWinnerList.RemoveWinner(nomineeToDisqualify);
            }

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
