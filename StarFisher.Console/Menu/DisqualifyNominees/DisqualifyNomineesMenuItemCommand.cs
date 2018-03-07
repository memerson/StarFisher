using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.DisqualifyNominees.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.DisqualifyNominees
{
    public class DisqualifyNomineesMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Disqualify a nominee";

        public DisqualifyNomineesMenuItemCommand(IStarFisherContext context)
            : base(context, CommandTitle)
        {
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            do
            {
                if (!TryGetAwardType(out AwardType awardType))
                    return CommandOutput.None.Success;

                if (!TryDisqualifyNominee(awardType))
                    return CommandOutput.None.Success;

            } while (GetDisqualifyAnotherNominee());

            return CommandOutput.None.Success;
        }

        private bool TryGetAwardType(out AwardType awardType)
        {
            var awardCategory = Context.NominationListContext.NominationList.AwardCategory;
            var parameter = new AwardTypeParameter(awardCategory);
            return TryGetArgumentValue(parameter, out awardType);
        }

        private bool TryDisqualifyNominee(AwardType awardType)
        {
            var nominationList = Context.NominationListContext.NominationList;
            var nominees = nominationList.GetNomineesForAward(awardType, false);
            var parameter = new NomineeToDisqualifyParameter(nominees);

            if (!TryGetArgumentValue(parameter, out Person nomineeToDisqualify))
                return false;

            nominationList.DisqualifyNominee(awardType, nomineeToDisqualify);
            return true;
        }

        private static bool GetDisqualifyAnotherNominee()
        {
            return TryGetArgumentValue(new DisqualifyAnotherNomineeParameter(), out bool disqualifyAnother) &&
                   disqualifyAnother;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized && Context.NominationListContext.HasNominationListLoaded;
        }
    }
}