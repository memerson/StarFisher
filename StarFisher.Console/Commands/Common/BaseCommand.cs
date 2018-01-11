using System;
using System.Text.RegularExpressions;

namespace StarFisher.Console.Commands.Common
{
    public abstract class BaseCommand : ICommand
    {
        private readonly Regex _commandRegex;
        private readonly bool _requiresNominationList;

        protected BaseCommand(Regex commandRegex, bool requiresNominationList)
        {
            _commandRegex = commandRegex ?? throw new ArgumentNullException(nameof(commandRegex));
            _requiresNominationList = requiresNominationList;
        }

        public CommandResult TryExecute(string commandText)
        {
            if (!_commandRegex.IsMatch(commandText))
                return CommandResult.NotAttempted;

            if (_requiresNominationList && !StarFisherContext.Current.HasNominationListLoaded)
                return CommandResult.Error("You must load a nomination list before executing this command.");

            try
            {
                var match = _commandRegex.Match(commandText);
                return TryExecute(match);
            }
            catch (Exception exception)
            {
                if (TryHandleException(exception, out string errorText))
                    return CommandResult.Error(errorText);

                return CommandResult.Error(exception.Message);
            }
        }

        protected abstract CommandResult TryExecute(Match commandRegexMatch);

        protected virtual bool TryHandleException(Exception exception, out string errorText)
        {
            errorText = null;
            return false;
        }
    }
}
