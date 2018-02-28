using System;
using System.IO;
using System.Reflection;
using StarFisher.Domain.NominationListAggregate;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    public interface IStarAwardsMemoHelper
    {
        void SaveArtifacts(WorkingDirectoryPath workingDirectoryPath, NominationList nominationList);
    }

    public class StarAwardsMemoHelper : IStarAwardsMemoHelper
    {
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly IMailMergeFactory _mailMergeFactory;

        public StarAwardsMemoHelper(IExcelFileFactory excelFileFactory, IMailMergeFactory mailMergeFactory)
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _mailMergeFactory = mailMergeFactory ?? throw new ArgumentNullException(nameof(mailMergeFactory));
        }

        public void SaveArtifacts(WorkingDirectoryPath workingDirectoryPath, NominationList nominationList)
        {
            if (workingDirectoryPath == null)
                throw new ArgumentNullException(nameof(workingDirectoryPath));
            if (nominationList == null)
                throw new ArgumentNullException(nameof(nominationList));

            SaveMemoSkeleton(workingDirectoryPath);
            SaveStarValuesWinnersMemoMailMerge(workingDirectoryPath, nominationList);
            SaveStarValuesNomineeListExcelFile(workingDirectoryPath, nominationList);
            SaveRisingStarWinnerMemoMailMerge(workingDirectoryPath, nominationList);
        }

        private static void SaveMemoSkeleton(WorkingDirectoryPath workingDirectoryPath)
        {
            const string memoSkeletonFileName = @"StarAwardWinnersAndNomineesForDistribution.docx";
            var memoSkeletonResourceName = $@"StarFisher.Office.Word.Documents.{memoSkeletonFileName}";

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(memoSkeletonResourceName);
            var filePath = workingDirectoryPath.GetFilePathForFileInDirectory(memoSkeletonFileName, false, false);

            using (stream)
            using (var tmpFileStream = File.Create(filePath.Value))
            {
                stream.CopyTo(tmpFileStream);
            }
        }

        private void SaveStarValuesWinnersMemoMailMerge(WorkingDirectoryPath workingDirectoryPath, NominationList nominationList)
        {
            if (!nominationList.HasStarValuesAwardWinners)
                return;

            var fileName = $@"{nominationList.AwardsPeriod.FileNamePrefix}_StarValuesWinnersForMemo.docx";
            var filePath = workingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);
            var mailMerge = _mailMergeFactory.GetStarValuesWinnersMemoMailMerge(nominationList);
            mailMerge.Execute(filePath);
        }

        private void SaveStarValuesNomineeListExcelFile(WorkingDirectoryPath workingDirectoryPath, NominationList nominationList)
        {
            if (!nominationList.HasStarValuesAwardWinners)
                return;

            var fileName = $@"{nominationList.AwardsPeriod.FileNamePrefix}_StarValuesNomineesForMemo.xlsx";
            var filePath = workingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);
            using (var excelFile = _excelFileFactory.GetStarValuesNomineeListExcelFile(nominationList))
                excelFile.Save(filePath);
        }

        private void SaveRisingStarWinnerMemoMailMerge(WorkingDirectoryPath workingDirectoryPath,
            NominationList nominationList)
        {
            if (!nominationList.HasRisingStarAwardWinners)
                return;

            var fileName = $@"{nominationList.AwardsPeriod.FileNamePrefix}_RisingStarWinnersForMemo.docx";
            var filePath = workingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);
            var mailMerge = _mailMergeFactory.GetRisingStarWinnersMemoMailMerge(nominationList);
            mailMerge.Execute(filePath);
        }
    }
}
