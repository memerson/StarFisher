
namespace StarFisher.Console.Menu.Common.Parameters
{
    public abstract class YesOrNoParameterBase : ParameterBase<bool>
    {
        protected YesOrNoParameterBase()
        {
            RegisterValidInput(@"yes", true);
            RegisterValidInput(@"no", false);
        }

        public override Argument<bool> GetArgumentCore()
        {
            WriteLine();
            WriteCallToAction(GetCallToActionText());
            WriteInputPrompt();

            return GetRegisteredValidInputArgument();
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid option.");
        }

        protected abstract string GetCallToActionText();
    }
}
