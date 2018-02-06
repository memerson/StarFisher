using System;
using System.Collections.Generic;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Commands
{
    public class FixNomineeEmailAddressesCommand : CommandBase<FixNomineeEmailAddressesCommand.Input, CommandOutput.None>
    {
        private const string CommandTitle = @"Fix incorrect nominee email addresses";

        public FixNomineeEmailAddressesCommand() : base(CommandTitle) { }

        public FixNomineeEmailAddressesCommand(IStarFisherContext context) : base(context, CommandTitle) { }

        protected override CommandResult<CommandOutput.None> RunCore(Input input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var nominationList = input.NominationList;
            var unrecognizedEmailAddresses = input.UnrecognizedEmailAddresses;

            for (; ; )
            {
                var nomineeParameter = new NomineeToChangeEmailAddressParameter(nominationList.Nominees, unrecognizedEmailAddresses);

                if (!TryGetArgumentValue(nomineeParameter, out Person nomineeToChange))
                    break;

                var newNomineeNameParameter = new NewNomineeEmailAddressParameter(nomineeToChange);
                if (!TryGetArgumentValue(newNomineeNameParameter, out EmailAddress newEmailAddress))
                    continue;

                nominationList.UpdateNomineeEmailAddress(nomineeToChange, newEmailAddress);
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
            public Input(NominationList nominationList, IReadOnlyCollection<EmailAddress> unrecognizedEmailAddresses)
            {
                NominationList = nominationList ?? throw new ArgumentNullException(nameof(nominationList));
                UnrecognizedEmailAddresses = unrecognizedEmailAddresses ?? throw new ArgumentNullException(nameof(unrecognizedEmailAddresses));
            }

            public NominationList NominationList { get; }

            public IReadOnlyCollection<EmailAddress> UnrecognizedEmailAddresses { get; }
        }
    }
}
