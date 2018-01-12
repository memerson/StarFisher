﻿using System;
using Microsoft.Office.Interop.Word;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Office.Excel;

namespace StarFisher.Office.Word
{
    public class StarValuesVotingGuideMailMerge : MailMergeBase
    {
        private readonly IExcelFileFactory _excelFileFactory;
        private readonly NominationList _nominationList;

        public StarValuesVotingGuideMailMerge(IExcelFileFactory excelFileFactory, NominationList nominationList)
            : base(@"StarFisher.Office.Word.MailMergeTemplates.StarValuesVotingGuideMailMergeTemplate.docx", WdMailMergeMainDocType.wdFormLetters)
        {
            _excelFileFactory = excelFileFactory ?? throw new ArgumentNullException(nameof(excelFileFactory));
            _nominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
        }

        protected override IExcelFile GetDataSourceExcelFile()
        {
            return _excelFileFactory.GetStarValuesVotingGuideSourceExcelFile(_nominationList);
        }
    }
}