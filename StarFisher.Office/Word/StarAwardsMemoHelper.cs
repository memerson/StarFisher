﻿using System;
using System.IO;
using System.Reflection;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    public interface IStarAwardsMemoHelper
    {
        void SaveArtifacts(DirectoryPath workingDirectoryPath, NominationList nominationList);
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

        public void SaveArtifacts(DirectoryPath workingDirectoryPath, NominationList nominationList)
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

        private static void SaveMemoSkeleton(DirectoryPath workingDirectoryPath)
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

        private void SaveStarValuesWinnersMemoMailMerge(DirectoryPath workingDirectoryPath, NominationList nominationList)
        {
            if (!nominationList.HasStarValuesAwardWinners)
                return;

            var fileName = $@"{nominationList.Year}{nominationList.Quarter.Abbreviation}StarValuesWinnersForMemo.docx";
            var filePath = workingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);
            var mailMerge = _mailMergeFactory.GetStarValuesWinnersMemoMailMerge(nominationList);
            mailMerge.Execute(filePath);
        }

        private void SaveStarValuesNomineeListExcelFile(DirectoryPath workingDirectoryPath, NominationList nominationList)
        {
            if (!nominationList.HasStarValuesAwardWinners)
                return;

            var fileName = $@"{nominationList.Year}{nominationList.Quarter.Abbreviation}StarValuesNomineesForMemo.xlsx";
            var filePath = workingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);
            using (var excelFile = _excelFileFactory.GetStarValuesNomineeListExcelFile(nominationList))
                excelFile.Save(filePath);
        }

        private void SaveRisingStarWinnerMemoMailMerge(DirectoryPath workingDirectoryPath,
            NominationList nominationList)
        {
            if (!nominationList.HasRisingStarAwardWinners)
                return;

            var fileName = $@"{nominationList.Year}{nominationList.Quarter.Abbreviation}RisingStarWinnersForMemo.docx";
            var filePath = workingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);
            var mailMerge = _mailMergeFactory.GetRisingStarWinnersMemoMailMerge(nominationList);
            mailMerge.Execute(filePath);
        }
    }
}
