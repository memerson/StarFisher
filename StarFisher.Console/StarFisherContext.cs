using System;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console
{
    public interface IWorkingDirectoryContext
    {
        DirectoryPath WorkingDirectoryPath { get; }

        bool HasWorkingDirectoryPathSet { get; }
    }

    public interface IStarFisherContext : IWorkingDirectoryContext
    {
        void SetContextNominationList(NominationList nominationList);

        void SetWorkingDirectoryPath(DirectoryPath directoryPath);

        NominationList NominationList { get; }

        bool HasNominationListLoaded { get; }
    }

    public class StarFisherContext : IStarFisherContext
    {
        public static readonly StarFisherContext Current = new StarFisherContext();

        private StarFisherContext() { }

        public void SetContextNominationList(NominationList nominationList)
        {
            NominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
        }

        public void SetWorkingDirectoryPath(DirectoryPath directoryPath)
        {
            WorkingDirectoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
        }

        public NominationList NominationList { get; private set; }

        public bool HasNominationListLoaded => NominationList != null;

        public DirectoryPath WorkingDirectoryPath { get; private set; }

        public bool HasWorkingDirectoryPathSet => WorkingDirectoryPath != null;
    }
}
