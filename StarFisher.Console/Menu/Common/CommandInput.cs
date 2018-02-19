namespace StarFisher.Console.Menu.Common
{
    public abstract class CommandInput
    {
        public sealed class None : CommandInput
        {
            public static readonly None Instance = new None();

            private None()
            {
            }
        }
    }
}