namespace StarFisher.Console.Menu.Common
{
    public interface ICommand<in TInput, TOutput>
        where TInput: CommandInput
        where TOutput: CommandOutput
    {
        string Title { get; }

        CommandResult<TOutput> Run(TInput input);
    }
}
