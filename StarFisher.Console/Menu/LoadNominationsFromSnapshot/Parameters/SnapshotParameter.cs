using System;
using System.Collections.Generic;
using StarFisher.Console.Menu.Common;
using StarFisher.Domain.QuarterlyAwards;

namespace StarFisher.Console.Menu.LoadNominationsFromSnapshot.Parameters
{
    public class SnapshotParameter : ParameterBase<SnapshotSummary>
    {
        private readonly IReadOnlyList<SnapshotSummary> _snapshotSummaries;

        public SnapshotParameter(IReadOnlyList<SnapshotSummary> snapshotSummaries)
        {
            _snapshotSummaries = snapshotSummaries ?? throw new ArgumentNullException(nameof(snapshotSummaries));
            RegisterAbortInput(@"stop");
        }

        public override Argument<SnapshotSummary> GetArgument()
        {
            if (_snapshotSummaries.Count == 0)
            {
                WriteLine(@"There are no snapshots to load.");
                return Argument<SnapshotSummary>.Abort;
            }

            WriteLine();
            WriteLineBlue(@"Here are the snapshot descriptions");
            WriteLine();
            WriteLine();

            for (var i = 0; i < _snapshotSummaries.Count; ++i)
            {
                if (i != 0 && i % 20 == 0)
                {
                    Write(@"Press any key to continue.");
                    WaitForKeyPress();
                    ClearLastLine();
                }

                var snapshotSummary = _snapshotSummaries[i];
                WriteLine($@"{i + 1,5}: [{snapshotSummary.DateTime}] Last Action: {snapshotSummary.LastChangeSummary}");
            }

            WriteLine();
            WriteLine(@"Enter the number of the snapshot you want to load, or enter 'stop' to stop loading nominations.");
            Write(@"> ");

            return GetArgumentFromInputIfValid();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid selection. I'm looking for one of the numbers next to one of snapshot descriptions.");
        }

        protected override bool TryParseArgumentValueFromInput(string input, out SnapshotSummary argumentValue)
        {
            if (int.TryParse(input, out int id))
            {
                var index = id - 1;
                if (index >= 0 && index < _snapshotSummaries.Count)
                {
                    argumentValue = _snapshotSummaries[index];
                    return true;
                }
            }

            argumentValue = null;
            return false;
        }
    }
}
