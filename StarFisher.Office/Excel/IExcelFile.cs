using System;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Office.Excel
{
    public interface IExcelFile : IDisposable
    {
        void Save(FilePath filePath);
    }
}
