using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFisher.Domain.Utilities
{
    public static class SafeMaxCollectionExtension
    {
        public static TResult SafeMax<T,TResult>(this IEnumerable<T> items, Func<T,TResult> selector)
        {
            return items == null ? default(TResult) : items.Select(selector).DefaultIfEmpty().Max();
        }
    }
}
