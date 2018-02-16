namespace StarFisher.Console.Menu.Common
{
    public interface IParameter<T>
    {
        Argument<T> GetValidArgument();

        Argument<T> GetArgumentCore();

        void PrintInvalidArgumentMessage();
    }
}
