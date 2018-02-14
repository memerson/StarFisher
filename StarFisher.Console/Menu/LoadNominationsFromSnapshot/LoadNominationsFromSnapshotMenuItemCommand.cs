using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.LoadNominationsFromSnapshot.Parameters;
using StarFisher.Domain.QuarterlyAwards;

namespace StarFisher.Console.Menu.LoadNominationsFromSnapshot
{
    public class LoadNominationsFromSnapshotMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Recover nominations from your previous work";

        public LoadNominationsFromSnapshotMenuItemCommand(IStarFisherContext context) : base(context, CommandTitle) { }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            if (!TryGetUseLatestSnapshot(out bool useLatestSnapshot))
                return CommandOutput.None.Abort;

            if (useLatestSnapshot)
            {
                Context.NominationListContext.LoadLatestSnapshot();
                return CommandOutput.None.Success;
            }

            if(!TryGetSnapshotToLoad(out SnapshotSummary snapshotSummary))
                return CommandOutput.None.Abort;

            Context.NominationListContext.LoadSnapshot(snapshotSummary);
            return CommandOutput.None.Success;
        }

        private static bool TryGetUseLatestSnapshot(out bool useLatestSnapshot)
        {
            var parameter = new LoadLatestSnapshotParameter();
            return TryGetArgumentValue(parameter, out useLatestSnapshot);
        }

        private bool TryGetSnapshotToLoad( out SnapshotSummary snapshotSummary)
        {
            var snapshotSummaries = Context.NominationListContext.ListSnapshotSummaries();
            var parameter = new SnapshotParameter(snapshotSummaries);
            return TryGetArgumentValue(parameter, out snapshotSummary);
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized && Context.NominationListContext.GetSnapshotCount() > 0;
        }
    }
}
