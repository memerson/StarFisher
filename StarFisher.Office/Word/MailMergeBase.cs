using System;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Word;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Excel;
using StarFisher.Office.Utilities;

namespace StarFisher.Office.Word
{
    internal abstract class MailMergeBase : IMailMerge
    {
        private readonly WdMailMergeMainDocType _mailMergeDocType;
        private readonly string _mailMergeTemplateResourceName;

        protected MailMergeBase(string mailMergeTemplateResourceName, WdMailMergeMainDocType mailMergeDocType)
        {
            if (string.IsNullOrWhiteSpace(mailMergeTemplateResourceName))
                throw new ArgumentException(nameof(mailMergeTemplateResourceName));

            _mailMergeTemplateResourceName = mailMergeTemplateResourceName;
            _mailMergeDocType = mailMergeDocType;
        }

        public void Execute()
        {
            using (var com = new ComObjectManager())
            {
                Execute(com, null);
            }
        }

        public void Execute(FilePath filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            using (var com = new ComObjectManager())
            {
                Execute(com, filePath);
            }
        }

        private void Execute(ComObjectManager com, FilePath outputFilePath)
        {
            object no = false;
            object missing = Missing.Value;
            var dataSourcePath = GetDataSourcePath();
            object templatePath = ExtractMailMergeTemplate(_mailMergeTemplateResourceName);
            var word = com.Get(() => new Application {Visible = false});
            var documents = com.Get(() => word.Documents);
            var mergeTemplateDocument =
                com.Get(() => documents.Add(ref templatePath, ref missing, ref missing, ref no));

            try
            {
                Execute(com, word, dataSourcePath, mergeTemplateDocument, outputFilePath);
            }
            finally
            {
                Cleanup(word, mergeTemplateDocument, dataSourcePath.Value, templatePath.ToString());
            }
        }

        private void Execute(ComObjectManager com, Application word, FilePath dataSourcePath,
            Document mergeTemplateDocument, FilePath outputFilePath)
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

            mailMerge.OpenDataSource(dataSourcePath.Value, ref format, ref no, ref no, ref yes,
                ref no,
                ref missing, ref missing, ref no, ref missing, ref missing, ref connection, ref sqlStatement,
                ref missing, ref no, ref subtype);

            DoMergeTypeSetup(com, mailMerge);

            mailMerge.SuppressBlankLines = true;

            var dataSource = com.Get(() => mailMerge.DataSource);
            dataSource.FirstRecord = (int) WdMailMergeDefaultRecord.wdDefaultFirstRecord;
            dataSource.LastRecord = (int) WdMailMergeDefaultRecord.wdDefaultLastRecord;

            mailMerge.Execute(ref no);

            if (outputFilePath != null)
                SaveMailMergeOutputDocument(com, word, outputFilePath, no);
        }

        private static void SaveMailMergeOutputDocument(ComObjectManager com, Application word, FilePath outputFilePath,
            object no)
        {
            var activeDocument = com.Get(() => word.ActiveDocument);
            object filePath = outputFilePath.Value;
            var extension = Path.GetExtension(outputFilePath.Value);

            var saveFormat = string.Equals(@".pdf", extension, StringComparison.InvariantCultureIgnoreCase)
                ? WdSaveFormat.wdFormatPDF
                : WdSaveFormat.wdFormatDocumentDefault;

            activeDocument.SaveAs(ref filePath, saveFormat);
            activeDocument.Close(ref no);
        }

        private static void Cleanup(Application word, Document mergeTemplateDocument, string dataSourcePath,
            string templatePath)
        {
            if (mergeTemplateDocument != null)
            {
                object no = false;
                mergeTemplateDocument.Close(ref no);
            }

            word.Quit();

            if (File.Exists(dataSourcePath))
                File.Delete(dataSourcePath);

            if (File.Exists(templatePath))
                File.Delete(templatePath);
        }

        protected abstract IExcelFile GetDataSourceExcelFile();

        protected abstract void DoMergeTypeSetup(ComObjectManager com, MailMerge mailMerge);

        private static string ExtractMailMergeTemplate(string mailMergeTemplateResourceName)
        {
            var tempFileName = Path.GetTempFileName() + ".docx";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(mailMergeTemplateResourceName);

            using (stream)
            using (var tmpFileStream = File.Create(tempFileName))
            {
                stream.CopyTo(tmpFileStream);
            }

            return tempFileName;
        }

        private FilePath GetDataSourcePath()
        {
            var dataSourcePath = FilePath.Create(Path.GetTempFileName() + ".xlsx", false);

            using (var excelFile = GetDataSourceExcelFile())
            {
                excelFile.Save(dataSourcePath);
            }

            return dataSourcePath;
        }
    }
}