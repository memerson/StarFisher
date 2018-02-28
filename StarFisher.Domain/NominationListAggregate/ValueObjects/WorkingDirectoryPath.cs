using System;
using System.IO;

namespace StarFisher.Domain.NominationListAggregate.ValueObjects
{
    public class WorkingDirectoryPath : DirectoryPath
    {
        private WorkingDirectoryPath(DirectoryPath directoryPath, AwardsPeriod awardsPeriod)
            : base(GetDirectoryPath(directoryPath, awardsPeriod))
        {
            AwardsPeriod = awardsPeriod;
        }

        public AwardsPeriod AwardsPeriod { get; }

        internal static WorkingDirectoryPath Create(DirectoryPath directoryPath, AwardsPeriod awardsPeriod)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));
            if (awardsPeriod == null)
                throw new ArgumentNullException(nameof(awardsPeriod));

            return new WorkingDirectoryPath(directoryPath, awardsPeriod);
        }

        private static string GetDirectoryPath(DirectoryPath directoryPath, AwardsPeriod awardsPeriod)
        {
            return Path.Combine(directoryPath.Value, awardsPeriod.FilePathFragment);
        }
    }
}