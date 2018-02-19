using System;
using StarFisher.Console.Menu.Common.Parameters;

namespace StarFisher.Console.Menu.Initialize.Parameters
{
    public class UseCurrentValueParameter : YesOrNoParameterBase
    {
        private readonly string _currentSettingValue;
        private readonly string _settingName;

        public UseCurrentValueParameter(string settingName, string currentSettingValue)
        {
            if (string.IsNullOrWhiteSpace(settingName))
                throw new ArgumentException(nameof(settingName));
            if (string.IsNullOrWhiteSpace(currentSettingValue))
                throw new ArgumentException(nameof(currentSettingValue));

            _settingName = settingName;
            _currentSettingValue = currentSettingValue;

            RegisterAbortInput(@"stop");
        }

        protected override string GetCallToActionText()
        {
            return $@"Your current value for {_settingName} is {
                    _currentSettingValue
                }. Would you like to keep that ('yes' or 'no')? You can also enter 'stop' to stop the initialization workflow.";
        }
    }
}