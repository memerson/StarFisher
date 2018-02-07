using System;
using StarFisher.Console.Menu.Common;

namespace StarFisher.Console.Menu.Initialize.Parameters
{
    public class UseCurrentValueParameter : ParameterBase<bool>
    {
        private readonly string _settingName;
        private readonly string _currentSettingValue;

        public UseCurrentValueParameter(string settingName, string currentSettingValue)
        {
            if (string.IsNullOrWhiteSpace(settingName))
                throw new ArgumentException(nameof(settingName));
            if (string.IsNullOrWhiteSpace(currentSettingValue))
                throw new ArgumentException(nameof(currentSettingValue));

            _settingName = settingName;
            _currentSettingValue = currentSettingValue;

            RegisterValidInput(@"yes", true);
            RegisterValidInput(@"no", false);
        }

        public override Argument<bool> GetArgument()
        {
            WriteLine();
            WriteLine($@"Your current value for {_settingName} is {_currentSettingValue}");
            WriteLine(@"Would you like to keep that?");
            Write(@"(yes or no) > ");

            return GetRegisteredValidInputArgument();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid option.");
        }
    }
}
