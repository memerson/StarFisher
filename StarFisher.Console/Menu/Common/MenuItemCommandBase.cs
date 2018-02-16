using System;
using StarFisher.Console.Context;

namespace StarFisher.Console.Menu.Common
{
    public abstract class MenuItemCommandBase : CommandBase<CommandInput.None, CommandOutput.None>, IMenuItemCommand
    {
        private readonly string _successMessage;

        private readonly IStarFisherConsole _console = StarFisherConsole.Instance;

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
            _console.SetCurrentCommandTitle(Title);
            _console.ResetConsole();

            var output = Run(CommandInput.None.Instance);

            _console.ClearCurrentCommandTitle();

            switch (output.ResultType)
            {
                case CommandResultType.Success:
                    _console.SetSuccessMessage(_successMessage);
                    break;
                case CommandResultType.Failure:
                    _console.PrintException(output.Exception);
                    break;
            }
        }

        public abstract bool GetCanRun();
    }
}
