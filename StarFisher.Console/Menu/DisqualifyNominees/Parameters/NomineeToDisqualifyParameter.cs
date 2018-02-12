﻿using System.Collections.Generic;
using StarFisher.Console.Menu.Common.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.DisqualifyNominees.Parameters
{
    public class NomineeToDisqualifyParameter : NomineeParameterBase
    {
        public NomineeToDisqualifyParameter(IReadOnlyCollection<Person> allNominees) : base(allNominees)
        {
            RegisterAbortInput(@"stop");
        }

        protected override string GetListItemLabel(Person listItem)
        {
            return $@"{listItem.Name.FullName} ({listItem.OfficeLocation.ConciseName})";
        }

        protected override string GetSelectionInstructions()
        {
            return @"Enter the number of the nominee you want to disqualify, or enter 'stop' to stop disqualifying nominees.";
        }
    }
}