using System;

namespace StarFisher.Console.Menu.Common
{
    public abstract class MenuItemCommandBase<TInput> : CommandBase<TInput, CommandOutput.None>, IMenuItemCommand<TInput>
        where TInput : CommandInput
    {
        protected MenuItemCommandBase(string title) : base(title) { }

        public MenuItem GetMenuItem(TInput input)
        {
            bool RunCommand(out Exception exception)
            {
                var output = Run(input);

                if (output.ResultType == CommandResultType.Failure)
                {
                    exception = output.Exception;
                    return false;
                }

                exception = null;
                return true;
            }

            return new MenuItem(Title,  RunCommand);
        }
    }
}
