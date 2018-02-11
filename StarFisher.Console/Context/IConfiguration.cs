using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook;

namespace StarFisher.Console.Context
{
    public interface IConfiguration : IEmailConfiguration
    {
        DirectoryPath WorkingDirectoryPath { get; }

        Year Year { get; }

        Quarter Quarter { get; }
    }
}
