using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Outlook;

namespace StarFisher.Console.Context
{
    public interface IConfiguration : IEmailConfiguration
    {
        WorkingDirectoryPath WorkingDirectoryPath { get; }

        Year Year { get; }

        Quarter Quarter { get; }
    }
}