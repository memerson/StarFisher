namespace StarFisher.Console.Menu.Common
{
    public interface IMenuItemCommand : ICommand<CommandInput.None, CommandOutput.None>
    {
        string Title { get; }

        bool GetCanRun();

        void Run();
    }
}