using System.Collections.Generic;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.Common.Parameters
{
    public abstract class AwardTypeParameterBase : ListItemSelectionParameterBase<AwardType>
    {
        protected AwardTypeParameterBase() : base(AwardType.ValidAwardTypes, @"awards")
        {
        }

        protected override string GetListItemLabel(AwardType listItem)
        {
            return listItem.PrettyName;
        }

        protected abstract override void WriteCallToAction();
    }
}
