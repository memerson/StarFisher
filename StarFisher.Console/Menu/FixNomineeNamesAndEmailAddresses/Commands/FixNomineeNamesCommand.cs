using System;
using System.Collections.Generic;
using System.Linq;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.FixNomineeNamesAndEmailAddresses.Commands
{
    public class FixNomineeNamesCommand : ICommand<FixNomineeNamesCommand.Input, FixNomineeNamesCommand.Output>
    {
        public string Title => @"Fix incorrect nominee names";

        public CommandResult<Output> Run(Input input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var allNomineeNames = input.AllNomineeNames.OrderBy(n => n.FullName).ToList();
            var unrecognizedNomineeNames = input.UnrecognizedNomineeNames;
            var nameChanges = new List<Output.NomineeNameChange>();

            for (; ; )
            {
                var nomineeNameToChangeParameter =
                    new NomineeNameToChangeParameter(allNomineeNames, unrecognizedNomineeNames);
                if (!TryGetNomineeName(nomineeNameToChangeParameter, out PersonName oldNomineeName))
                    break;

                var newNomineeNameParameter = new NewNomineeNameParameter(oldNomineeName);
                if (!TryGetNomineeName(newNomineeNameParameter, out PersonName newNomineeName))
                    continue;

                var nameChange = new Output.NomineeNameChange(oldNomineeName, newNomineeName);
                nameChanges.Add(nameChange);

                allNomineeNames = allNomineeNames
                    .Select(n => n == oldNomineeName ? newNomineeName : n)
                    .OrderBy(n => n.FullName)
                    .ToList();
            }

            var output = new Output(nameChanges);
            return CommandResult<Output>.Success(output);
        }

        private static bool TryGetNomineeName(IParameter<PersonName> parameter, out PersonName nomineeName)
        {
            nomineeName = null;
            var argument = parameter.GetArgument();

            while (argument.ArgumentType == ArgumentType.Invalid ||
                   argument.ArgumentType == ArgumentType.NoValue)
            {
                parameter.PrintInvalidArgumentMessage();
                argument = parameter.GetArgument();
            }

            if (argument.ArgumentType == ArgumentType.Abort)
                return false;

            nomineeName = argument.Value;
            return true;
        }

        public class Input : CommandInput
        {
            public Input(IReadOnlyCollection<PersonName> allNomineeNames, IReadOnlyCollection<PersonName> unrecognizedNomineeNames)
            {
                AllNomineeNames = allNomineeNames ?? throw new ArgumentNullException(nameof(allNomineeNames));
                UnrecognizedNomineeNames = unrecognizedNomineeNames ?? throw new ArgumentNullException(nameof(unrecognizedNomineeNames));

                if (AllNomineeNames.Count == 0)
                    throw new ArgumentException(nameof(allNomineeNames));
            }

            public IReadOnlyCollection<PersonName> AllNomineeNames { get; }

            public IReadOnlyCollection<PersonName> UnrecognizedNomineeNames { get; }
        }

        public class Output : CommandOutput
        {
            public Output(IReadOnlyCollection<NomineeNameChange> nomineeNameChanges)
            {
                NomineeNameChanges = nomineeNameChanges ?? throw new ArgumentNullException(nameof(nomineeNameChanges));
            }

            public IReadOnlyCollection<NomineeNameChange> NomineeNameChanges { get; }

            public class NomineeNameChange
            {
                public NomineeNameChange(PersonName oldNomineeName, PersonName newNomineeName)
                {
                    OldNomineeName = oldNomineeName ?? throw new ArgumentNullException(nameof(oldNomineeName));
                    NewNomineeName = newNomineeName ?? throw new ArgumentNullException(nameof(newNomineeName));
                }

                public PersonName OldNomineeName { get; }

                public PersonName NewNomineeName { get; }
            }
        }
    }
}
