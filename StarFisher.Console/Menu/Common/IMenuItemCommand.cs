
namespace StarFisher.Console.Menu.Common
{
    public interface IMenuItemCommand<in TInput> : ICommand<TInput, CommandOutput.None>
        where TInput : CommandInput
    {
        MenuItem GetMenuItem(TInput input);
    }
}
