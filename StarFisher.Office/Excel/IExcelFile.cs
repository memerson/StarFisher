using System;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Office.Excel
{
    public interface IExcelFile : IDisposable
    {
        void Save(FilePath filePath);
    }
}