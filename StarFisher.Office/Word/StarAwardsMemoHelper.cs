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
            SaveNomineeListExcelFile(workingDirectoryPath, nominationList, AwardType.StarValues);
            SaveRisingStarWinnerMemoMailMerge(workingDirectoryPath, nominationList);
            SaveNomineeListExcelFile(workingDirectoryPath, nominationList, AwardType.RisingStar);
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

        private void SaveStarValuesWinnersMemoMailMerge(WorkingDirectoryPath workingDirectoryPath,
            NominationList nominationList)
        {
            var mailMerge = _mailMergeFactory.GetStarValuesWinnersMemoMailMerge(nominationList);
            SaveAwardWinnerMemoMailMerge(workingDirectoryPath, AwardType.StarValues, nominationList, mailMerge);
        }

        private void SaveNomineeListExcelFile(WorkingDirectoryPath workingDirectoryPath, NominationList nominationList,
            AwardType awardType)
        {
            if (!nominationList.HasNominationsForAward(awardType))
                return;

            var fileName = awardType.GetNomineesForMemoFileName(nominationList.AwardsPeriod);
            var filePath = workingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);
            using (var excelFile = _excelFileFactory.GetNomineeListExcelFile(awardType, nominationList))
            {
                excelFile.Save(filePath);
            }
        }

        private void SaveRisingStarWinnerMemoMailMerge(WorkingDirectoryPath workingDirectoryPath,
            NominationList nominationList)
        {
            var mailMerge = _mailMergeFactory.GetRisingStarWinnersMemoMailMerge(nominationList);
            SaveAwardWinnerMemoMailMerge(workingDirectoryPath, AwardType.RisingStar, nominationList, mailMerge);
        }

        private static void SaveAwardWinnerMemoMailMerge(WorkingDirectoryPath workingDirectoryPath, AwardType awardType,
            NominationList nominationList, IMailMerge mailMerge)
        {
            if (!nominationList.HasWinnersForAward(awardType))
                return;

            var fileName = awardType.GetWinnersForMemoFileName(nominationList.AwardsPeriod);
            var filePath = workingDirectoryPath.GetFilePathForFileInDirectory(fileName, false, false);

            mailMerge.Execute(filePath);
        }
    }
}