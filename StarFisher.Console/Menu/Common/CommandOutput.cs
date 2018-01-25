
namespace StarFisher.Console.Menu.Common
{
    public abstract class CommandOutput
    {
        public sealed class None : CommandOutput
        {
            public static readonly None Instance = new None();

            private None() { }
        }
    }
}
