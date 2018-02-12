using System.Collections.Generic;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.QuarterlyAwards;

namespace StarFisher.Console.Menu.LoadNominationsFromSnapshot.Parameters
{
    public class SnapshotParameter : ListItemSelectionParameterBase<SnapshotSummary>
    {
        public SnapshotParameter(IReadOnlyList<SnapshotSummary> snapshotSummaries)
            : base(snapshotSummaries, @"snapshots")
        {
            RegisterAbortInput(@"stop");
        }

        protected override string GetListItemLabel(SnapshotSummary listItem)
        {
            return $@"[{listItem.DateTime:MM/dd/yyyy hh:mm:ss tt}] Last Action: {listItem.LastChangeSummary}";
        }

        protected override string GetSelectionInstructions()
        {
            return @"Enter the number of the snapshot you want to load, or enter 'stop' to stop loading nominations.";
        }
    }
}
