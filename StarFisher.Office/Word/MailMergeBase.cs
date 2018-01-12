using System;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Word;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Word
{
    public abstract class MailMergeBase : IMailMerge
    {
        private readonly string _mailMergeTemplateResourceName;
        private readonly WdMailMergeMainDocType _mailMergeDocType;

        protected MailMergeBase(string mailMergeTemplateResourceName, WdMailMergeMainDocType mailMergeDocType)
        {
            if(string.IsNullOrWhiteSpace(mailMergeTemplateResourceName))
                throw new ArgumentException(nameof(mailMergeTemplateResourceName));

            _mailMergeTemplateResourceName = mailMergeTemplateResourceName;
            _mailMergeDocType = mailMergeDocType;
        }

        public void Execute()
        {
            using (var com = new ComObjectManager())
                Execute(com, _mailMergeTemplateResourceName);
        }

        private void Execute(ComObjectManager com, string mailMergeTemplateResourceName)
        {
            object no = false;
            object missing = Missing.Value;
            var dataSourcePath = GetDataSourcePath();
            object templatePath = ExtractMailMergeTemplate(mailMergeTemplateResourceName);
            var word = com.Get(() => new Application { Visible = true });
            var documents = com.Get(() => word.Documents);
            var mergeTemplateDocument = com.Get(() => documents.Add(ref templatePath, ref missing, ref missing, ref no));

            try
            {
                Execute(com, dataSourcePath, mergeTemplateDocument);
            }
            finally 
            {
                Cleanup(mergeTemplateDocument, dataSourcePath.Value, templatePath.ToString());
            }
        }

        private void Execute(ComObjectManager com, FilePath dataSourcePath, Document mergeTemplateDocument)
        {
            object yes = true;
            object no = false;
            object missing = Missing.Value;
            object format = WdOpenFormat.wdOpenFormatAuto;
            object subtype = WdMergeSubType.wdMergeSubTypeAccess;
            object connection =
                $"Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source={dataSourcePath.Value};Mode=Read;Extended Properties=\"HDR=YES;IMEX=1;\";Jet OLEDB:Engine Type=37;Jet OLEDB:Database ";
            object sqlStatement = @"SELECT * FROM `Sheet1$`";

            var mailMerge = com.Get(() => mergeTemplateDocument.MailMerge);

            mailMerge.MainDocumentType = _mailMergeDocType;

            mergeTemplateDocument.MailMerge.OpenDataSource(dataSourcePath.Value, ref format, ref no, ref no, ref yes, ref no,
                ref missing, ref missing, ref no, ref missing, ref missing, ref connection, ref sqlStatement,
                ref missing, ref no, ref subtype);

            mailMerge.Destination = WdMailMergeDestination.wdSendToNewDocument;
            mailMerge.SuppressBlankLines = true;

            var dataSource = com.Get(() => mailMerge.DataSource);
            dataSource.FirstRecord = (int) WdMailMergeDefaultRecord.wdDefaultFirstRecord;
            dataSource.LastRecord = (int) WdMailMergeDefaultRecord.wdDefaultLastRecord;

            mailMerge.Execute(ref no);
        }

        private static void Cleanup(Document mergeTemplateDocument, string dataSourcePath, string templatePath)
        {
            if (mergeTemplateDocument != null)
            {
                object no = false;
                object missing = Missing.Value;
                mergeTemplateDocument.Close(ref no, ref missing, ref missing);
            }

            if (File.Exists(dataSourcePath))
                File.Delete(dataSourcePath);

            if (File.Exists(templatePath))
                File.Delete(templatePath);
        }

        protected abstract IExcelFile GetDataSourceExcelFile();

        private static string ExtractMailMergeTemplate(string mailMergeTemplateResourceName)
        {
            var tempFileName = Path.GetTempFileName() + ".docx";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(mailMergeTemplateResourceName);

            using (stream)
            using (var tmpFileStream = File.Create(tempFileName))
                stream.CopyTo(tmpFileStream);

            return tempFileName;
        }

        private FilePath GetDataSourcePath()
        {
            var dataSourcePath = FilePath.Create(Path.GetTempFileName() + ".xlsx", false);

            using (var excelFile = GetDataSourceExcelFile())
                excelFile.Save(dataSourcePath);

            return dataSourcePath;
        }
    }
}
