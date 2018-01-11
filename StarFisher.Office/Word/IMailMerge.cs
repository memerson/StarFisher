using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Word
{
    public interface IMailMerge
    {
        void Execute(NominationList nominationList);
    }
}