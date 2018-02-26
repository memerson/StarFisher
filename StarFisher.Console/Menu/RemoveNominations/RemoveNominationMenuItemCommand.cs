using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.RemoveNominations.Parameters;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate.Entities;

namespace StarFisher.Console.Menu.RemoveNominations
{
    public class RemoveNominationMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Remove a nomination";

        public RemoveNominationMenuItemCommand(IStarFisherContext context) : base(context, CommandTitle)
        {
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            do
            {
                if (!RemoveNomination())
                    return CommandOutput.None.Success;
            } while (GetRemoveAnotherNomination());

            return CommandOutput.None.Success;
        }

        private bool RemoveNomination()
        {
            var nominationList = Context.NominationListContext.NominationList;
            var parameter = new NominationToRemoveParameter(nominationList.Nominations);
            if (!TryGetArgumentValue(parameter, out Nomination nominationToRemove))
                return false;

            if (!nominationList.GetIsOnlyNominationForAwardAndNominee(nominationToRemove) ||
                RemoveOnlyNomination(nominationToRemove))
            {
                nominationList.RemoveNomination(nominationToRemove.Id);
            }

            return true;
        }

        private bool RemoveOnlyNomination(Nomination nomination)
        {
            var parameter = new RemoveOnlyNominationParameter(nomination);
            return TryGetArgumentValue(parameter, out bool removeNomination) && removeNomination;
        }

        private static bool GetRemoveAnotherNomination()
        {
            return TryGetArgumentValue(new RemoveAnotherNominationParameter(), out bool removeAnother) && removeAnother;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized && Context.NominationListContext.HasNominationListLoaded;
        }
    }
}