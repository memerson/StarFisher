using System.Collections.Generic;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.Common.Parameters
{
    public abstract class AwardTypeParameterBase : ListItemSelectionParameterBase<AwardType>
    {
        private static readonly IReadOnlyList<AwardType> AwardTypes = new List<AwardType>
        {
            AwardType.RisingStar,
            AwardType.StarValues
        };

        protected AwardTypeParameterBase() : base(AwardTypes, @"awards")
        {
        }

        protected override string GetListItemLabel(AwardType listItem)
        {
            return listItem.PrettyName;
        }
    }
}
