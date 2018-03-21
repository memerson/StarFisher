using System;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.FixNominees.Parameters
{
    public class NewNomineeOfficeLocationParameter : ListItemSelectionParameterBase<OfficeLocation>
    {
        private readonly PersonName _nomineeName;

        public NewNomineeOfficeLocationParameter(PersonName nomineeName) : base(
            OfficeLocation.ValidEmployeeOfficeLocations, @"office locations")
        {
            _nomineeName = nomineeName ?? throw new ArgumentNullException(nameof(nomineeName));

            RegisterAbortInput(@"done");
        }

        protected override string GetListItemLabel(OfficeLocation listItem)
        {
            return listItem.Name;
        }

        protected override void WriteCallToAction()
        {
            WriteCallToAction(
                $@"Enter the number of the correct office location for {
                        _nomineeName.FullName
                    }, or enter 'done' if you don't want to change it.");
        }
    }
}