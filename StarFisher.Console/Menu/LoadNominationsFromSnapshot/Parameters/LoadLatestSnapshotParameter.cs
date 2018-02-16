using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Common.Parameters;

namespace StarFisher.Console.Menu.LoadNominationsFromSnapshot.Parameters
{
    public class LoadLatestSnapshotParameter : YesOrNoParameterBase
    {
        public LoadLatestSnapshotParameter()
        {
            RegisterAbortInput(@"stop");
        }

        protected override string GetCallToActionText()
        {
            return @"Would you like to pick up where you last left off with the nominations ('yes' or 'no')? You can also enter 'stop' to stop loading nominations.";
        }
    }
}
