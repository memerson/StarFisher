
namespace StarFisher.Console.Commands.Common
{
    public interface ICommand
    {
        CommandResult TryExecute(string commandText);
    }
}
