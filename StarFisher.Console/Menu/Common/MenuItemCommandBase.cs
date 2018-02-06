using System;
using StarFisher.Console.Context;
using StarFisher.Console.Utilities;

namespace StarFisher.Console.Menu.Common
{
    public abstract class MenuItemCommandBase : CommandBase<CommandInput.None, CommandOutput.None>, IMenuItemCommand
    {
        protected MenuItemCommandBase(IStarFisherContext context, string title) : base(context, title) { }

        protected MenuItemCommandBase(string title) : base(title) { }

        public void Run()
        {
            if (!Run(out Exception exception))
                PrintException(exception);
        }

        private bool Run(out Exception exception)
        {
            var output = Run(CommandInput.None.Instance);

            if (output.ResultType == CommandResultType.Failure)
            {
                exception = output.Exception;
                return false;
            }

            exception = null;
            return true;
        }

        private static void PrintException(Exception exception)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.Red))
            {
                System.Console.WriteLine();
                System.Console.WriteLine(exception?.ToString() ?? "UNKNOWN ERROR");
                System.Console.WriteLine();
            }
        }

        public abstract bool GetCanRun();
    }
}
