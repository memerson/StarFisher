using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Outlook;

namespace StarFisher.Console.Context
{
    public interface IConfiguration : IEmailConfiguration
    {
        WorkingDirectoryPath WorkingDirectoryPath { get; }

        AwardsPeriod AwardsPeriod { get; }
    }
}