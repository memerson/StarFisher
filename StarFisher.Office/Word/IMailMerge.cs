using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Word
{
    public interface IMailMerge
    {
        void Execute();
        void Execute(FilePath filePath);
    }
}