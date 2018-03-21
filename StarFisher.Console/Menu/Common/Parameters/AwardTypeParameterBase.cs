using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.Common.Parameters
{
    public abstract class AwardTypeParameterBase : ListItemSelectionParameterBase<AwardType>
    {
        protected AwardTypeParameterBase(AwardCategory awardCategory) : base(GetAwardTypes(awardCategory), @"awards")
        {
        }

        public override Argument<AwardType> GetArgumentCore()
        {
            if (ListItems.Count == 1)
                return Argument<AwardType>.Valid(ListItems[0]);

            return base.GetArgumentCore();
        }

        protected override string GetListItemLabel(AwardType listItem)
        {
            return listItem.PrettyName;
        }

        protected abstract override void WriteCallToAction();

        private static IReadOnlyList<AwardType> GetAwardTypes(AwardCategory awardCategory)
        {
            if (awardCategory == null)
                throw new ArgumentNullException(nameof(awardCategory));

            return AwardType.ValidAwardTypes
                .Where(at => at.AwardCategory == awardCategory)
                .ToList();
        }
    }
}