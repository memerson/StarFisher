using System;
using System.Collections.Generic;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Commands
{
    public class FixNomineeNamesCommand : CommandBase<FixNomineeNamesCommand.Input, CommandOutput.None>
    {
        private const string CommandTitle = @"Fix incorrect nominee names";

        public FixNomineeNamesCommand() : base(CommandTitle) { }

        public FixNomineeNamesCommand(IStarFisherContext context) : base(context, CommandTitle) { }

        protected override CommandResult<CommandOutput.None> RunCore(Input input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var nominationList = input.NominationList;
            var unrecognizedNomineeNames = input.UnrecognizedNomineeNames;

            for (; ; )
            {
                var nomineeParameter =
                    new NomineeToChangeNameParameter(nominationList.Nominees, unrecognizedNomineeNames);

                if (!TryGetArgumentValue(nomineeParameter, out Person nomineeToChange))
                    break;

                var newNomineeNameParameter = new NewNomineeNameParameter(nomineeToChange);
                if (!TryGetArgumentValue(newNomineeNameParameter, out PersonName newNomineeName))
                    continue;

                nominationList.UpdateNomineeName(nomineeToChange, newNomineeName);
            }

            return CommandOutput.None.Success;
        }

        private static bool TryGetArgumentValue<T>(IParameter<T> parameter, out T argumentValue)
        {
            argumentValue = default(T);
            var argument = parameter.GetArgument();

            while (argument.ArgumentType == ArgumentType.Invalid)
            {
                parameter.PrintInvalidArgumentMessage();
                argument = parameter.GetArgument();
            }

            if (argument.ArgumentType == ArgumentType.Abort ||
                argument.ArgumentType == ArgumentType.NoValue)
            {
                return false;
            }

            argumentValue = argument.Value;
            return true;
        }

        public class Input : CommandInput
        {
            public Input(NominationList nominationList, IReadOnlyCollection<PersonName> unrecognizedNomineeNames)
            {
                NominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
                UnrecognizedNomineeNames = unrecognizedNomineeNames ?? throw new ArgumentNullException(nameof(unrecognizedNomineeNames));
            }

            public NominationList NominationList { get; }

            public IReadOnlyCollection<PersonName> UnrecognizedNomineeNames { get; }
        }
    }
}
