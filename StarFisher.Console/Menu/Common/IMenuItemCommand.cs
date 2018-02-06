
namespace StarFisher.Console.Menu.Common
{
    public interface IMenuItemCommand : ICommand<CommandInput.None, CommandOutput.None>
    {
        bool GetCanRun();

        void Run();
    }
}
