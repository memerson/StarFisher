using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNominees.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;

namespace StarFisher.Console.Menu.FixNominees.Commands
{
    public class FixNomineeOfficeLocationsCommand : CommandBase<CommandInput.None, CommandOutput.None>
    {
        public FixNomineeOfficeLocationsCommand(IStarFisherContext context) : base(context)
        {
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var nominationList = Context.NominationListContext.NominationList;

            for (;;)
            {
                var nomineeParameter =
                    new NomineeToChangeOfficeLocationParameter(nominationList.Nominees);

                if (!TryGetArgumentValue(nomineeParameter, out Person nomineeToChange))
                    break;

                var newNomineeOfficeLocationParameter = new NewNomineeOfficeLocationParameter(nomineeToChange.Name);
                if (!TryGetArgumentValue(newNomineeOfficeLocationParameter,
                    out OfficeLocation newNomineeOfficeLocation))
                    continue;

                nominationList.UpdateNomineeOfficeLocation(nomineeToChange, newNomineeOfficeLocation);
            }

            return CommandOutput.None.Success;
        }
    }
}