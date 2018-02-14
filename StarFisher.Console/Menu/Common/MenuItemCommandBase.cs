using System;
using StarFisher.Console.Context;
using StarFisher.Console.Utilities;

namespace StarFisher.Console.Menu.Common
{
    public abstract class MenuItemCommandBase : CommandBase<CommandInput.None, CommandOutput.None>, IMenuItemCommand
    {
        private readonly string _successMessage;

        protected MenuItemCommandBase(IStarFisherContext context, string title,
            string successMessage = @"Operation completed successfully")
            : base(context)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException(nameof(title));
            if (string.IsNullOrEmpty(successMessage))
                throw new ArgumentException(nameof(successMessage));

            Title = title;
            _successMessage = successMessage;
        }

        protected MenuItemCommandBase(string title) : this(null, title) { }

        public string Title { get; }

        public void Run()
        {
            var output = Run(CommandInput.None.Instance);

            switch (output.ResultType)
            {
                case CommandResultType.Success:
                    PrintSuccessMessage();
                    break;
                case CommandResultType.Failure:
                    PrintException(output.Exception);
                    break;
            }
        }

        private void PrintSuccessMessage()
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.Green))
            {
                System.Console.WriteLine();
                System.Console.WriteLine(_successMessage);
            }
        }

        private static void PrintException(Exception exception)
        {
            using (ConsoleColorSelector.SetConsoleForegroundColor(ConsoleColor.Red))
            {
                System.Console.WriteLine();
                System.Console.WriteLine(@"The operation has failed.");
                System.Console.WriteLine();
                System.Console.WriteLine(exception?.ToString() ?? "UNKNOWN ERROR");
            }
        }

        public abstract bool GetCanRun();
    }
}
