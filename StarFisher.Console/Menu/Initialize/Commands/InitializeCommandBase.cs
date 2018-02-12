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
            if(!TryGetUseCurrentValueParameter(settingName, currentSettingValue, out bool useCurrentValue))
                return CommandResult<Output>.Abort();

            if(useCurrentValue)
                return CommandResult<Output>.Success(new Output(currentSettingValue));

            if(!TryGetArgumentValue(getNewValueParameter, out TValue newValue))
                return CommandResult<Output>.Abort();

            return CommandResult<Output>.Success(new Output(newValue));
        }

        private static bool TryGetUseCurrentValueParameter(string settingName, TValue currentSettingValue, out bool useCurrentValue)
        {
            var currentSettingValueText = currentSettingValue?.ToString();
            var hasCurrentSettingValue = !string.IsNullOrWhiteSpace(currentSettingValueText);

            if (!hasCurrentSettingValue)
            {
                useCurrentValue = false;
                return true;
            }

            var parameter = new UseCurrentValueParameter(settingName, currentSettingValueText);

            return TryGetArgumentValue(parameter, out useCurrentValue);
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
