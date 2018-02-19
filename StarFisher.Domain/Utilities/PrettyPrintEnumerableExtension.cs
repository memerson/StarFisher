using System.Collections;
using System.Text;

namespace StarFisher.Domain.Utilities
{
    public static class PrettyPrintEnumerableExtension
    {
        public static string PrettyPrint(this IEnumerable enumerable)
        {
            var stringBuilder = new StringBuilder();
            var enumerator = enumerable.GetEnumerator();

            if (enumerator.MoveNext())
                stringBuilder.Append(enumerator.Current);

            object item = null;

            if (enumerator.MoveNext())
                item = enumerator.Current;

            while (enumerator.MoveNext())
            {
                stringBuilder.Append(", ");
                stringBuilder.Append(item);
                item = enumerator.Current;
            }

            if (item == null)
                return stringBuilder.ToString();

            stringBuilder.Append(" and ");
            stringBuilder.Append(item);

            return stringBuilder.ToString();
        }
    }
}