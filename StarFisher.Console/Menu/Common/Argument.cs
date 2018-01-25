namespace StarFisher.Console.Menu.Common
{
    public class Argument<T>
    {
        public static readonly Argument<T> Invalid = new Argument<T>(ArgumentType.Invalid, default(T));

        public static readonly Argument<T> Abort = new Argument<T>(ArgumentType.Abort, default(T));

        public static readonly Argument<T> NoValue = new Argument<T>(ArgumentType.NoValue, default(T));

        private Argument(ArgumentType argumentType, T value)
        {
            Value = value;
            ArgumentType = argumentType;
        }

        public static Argument<T> Valid(T value)
        {
            return new Argument<T>(ArgumentType.Valid, value);
        }

        public T Value { get; }

        public ArgumentType ArgumentType { get; }
    }
}
