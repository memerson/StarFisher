using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.Initialize.Parameters;

namespace StarFisher.Console.Menu.Initialize.Commands
{
    public abstract class InitializeCommandBase<TValue> : InitializeCommandBase<CommandInput.None, TValue>
        where TValue : class
    {
        protected InitializeCommandBase(IStarFisherContext context) : base(context){ }
    }

    public abstract class InitializeCommandBase<TCommandInput, TValue> : CommandBase<TCommandInput, InitializeCommandBase<TCommandInput, TValue>.Output>
        where TCommandInput : CommandInput
        where TValue : class
    {
        protected InitializeCommandBase(IStarFisherContext context) : base(context) { }

        protected CommandResult<Output> RunCoreHelper(string settingName, TValue currentSettingValue,
            IParameter<TValue> getNewValueParameter)
        {
            var currentSettingValueText = currentSettingValue?.ToString();
            var useCurrentValue = GetUseCurrentValue(settingName, currentSettingValueText, out bool stop);

            if (stop)
                return CommandResult<Output>.Abort();

            return useCurrentValue
                ? CommandResult<Output>.Success(new Output(currentSettingValue))
                : GetNewValue(getNewValueParameter);
        }

        private bool GetUseCurrentValue(string settingName, string currentSettingValueText, out bool stop)
        {
            stop = false;

            if (string.IsNullOrWhiteSpace(currentSettingValueText) || !Context.IsInitialized)
                return false;

            var parameter = new UseCurrentValueParameter(settingName, currentSettingValueText);
            var argument = parameter.GetArgument();

            while (argument.ArgumentType == ArgumentType.Invalid)
            {
                parameter.PrintInvalidArgumentMessage();
                argument = parameter.GetArgument();
            }

            if (argument.ArgumentType != ArgumentType.Abort)
                return argument.ArgumentType == ArgumentType.Valid && argument.Value;

            stop = true;
            return false;
        }

        private CommandResult<Output> GetNewValue(IParameter<TValue> parameter)
        {
            var argument = parameter.GetArgument();

            while (argument.ArgumentType == ArgumentType.Invalid)
            {
                parameter.PrintInvalidArgumentMessage();
                argument = parameter.GetArgument();
            }

            if (argument.ArgumentType == ArgumentType.Abort)
                return CommandResult<Output>.Abort();

            return CommandResult<Output>.Success(new Output(argument.Value));
        }

        public class Output : CommandOutput
        {
            public Output(TValue value)
            {
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }

            public TValue Value { get; }
        }
    }
}
