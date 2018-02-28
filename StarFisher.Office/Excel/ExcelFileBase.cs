using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using StarFisher.Domain.NominationListAggregate.Entities;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Utilities;
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;

namespace StarFisher.Office.Excel
{
    internal abstract class ExcelFileBase : IExcelFile
    {
        private readonly Workbook _workbook;
        private bool _isDisposed;

        protected ExcelFileBase(Action<ComObjectManager, Worksheet> buildWorksheet)
        {
            if (buildWorksheet == null)
                throw new ArgumentNullException(nameof(buildWorksheet));

            ExcelApplication excel = null;

            try
            {
                excel = Com.Get(() => new ExcelApplication {Visible = false});
                _workbook = Com.Get(() => excel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet));
                var worksheet = Com.Get(() => (Worksheet) _workbook.Worksheets[1]);

                buildWorksheet(Com, worksheet);
            }
            catch
            {
                _workbook?.Close();
                excel?.Quit();
                Com.Dispose();

                throw;
            }
        }

        private ComObjectManager Com { get; } = new ComObjectManager();

        public virtual void Dispose()
        {
            if (_isDisposed)
                return;

            var excel = Com.Get(() => _workbook.Application);
            _workbook.Close();
            excel.Quit();

            _isDisposed = true;
            Com.Dispose();
        }

        public void Save(FilePath filePath)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("The object is disposed.");

            if (File.Exists(filePath.Value))
                File.Delete(filePath.Value);

            var missing = Missing.Value;

            _workbook.SaveAs(filePath.Value, XlFileFormat.xlOpenXMLWorkbook, missing, missing, false, false,
                XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing, missing);
        }

        protected static string GetCompanyValue(Nomination nomination, CompanyValue companyValue)
        {
            return nomination.CompanyValues.FirstOrDefault(cv => cv == companyValue)?.ToString() ??
                   string.Empty;
        }

        protected static void SetCellValue(Range cells, int row, int column, object value)
        {
            cells.set_Item(row, column, value);
        }
    }
}