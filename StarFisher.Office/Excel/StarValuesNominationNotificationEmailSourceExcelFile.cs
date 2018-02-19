﻿using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;

namespace StarFisher.Office.Excel
{
    internal class StarValuesNominationNotificationEmailSourceExcelFile : NominationNotificationEmailSourceExcelFileBase
    {
        public StarValuesNominationNotificationEmailSourceExcelFile(NominationList nominationList)
            : base(nominationList?.Quarter, nominationList?.StarValuesNominations)
        {
        }
    }
}