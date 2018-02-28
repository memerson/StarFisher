using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.Initialize.Parameters
{
    public class AwardCategoryParameter : ListItemSelectionParameterBase<AwardCategory>
    {
        public AwardCategoryParameter() : base(AwardCategory.ValidAwardsCategories, @"awards categories")
        {
        }

        protected override void WriteListIntroduction()
        {
            WriteIntroduction(@"On which category of awards will we work? Here are the award categories:");
        }

        protected override string GetListItemLabel(AwardCategory listItem)
        {
            return listItem.ToString();
        }

        protected override void WriteCallToAction()
        {
            WriteCallToAction(@"Enter the number of the award category on which you will work, or enter 'stop' to stop the initialization workflow.");
        }
    }
}
