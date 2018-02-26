using System;
using System.IO;

namespace StarFisher.Domain.ValueObjects
{
    public class WorkingDirectoryPath : DirectoryPath
    {
        private WorkingDirectoryPath(DirectoryPath directoryPath, Year year, Quarter quarter)
            : base(Path.Combine(directoryPath.Value, year.ToString(), quarter.Abbreviation))
        {
            Year = year;
            Quarter = quarter;
        }

        internal static WorkingDirectoryPath Create(DirectoryPath directoryPath, Year year, Quarter quarter)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));
            if (year == null)
                throw new ArgumentNullException(nameof(year));
            if (quarter == null)
                throw new ArgumentNullException(nameof(quarter));

            return new WorkingDirectoryPath(directoryPath, year, quarter);
        }

        public Year Year { get; }

        public Quarter Quarter { get; }
    }
}
