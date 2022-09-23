using System;
using System.Collections.Generic;
using System.Linq;

namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator.Extensions;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
    {
        return items.GroupBy(property).Select(x =>
            x.FirstOrDefault() ?? throw new ArgumentException($"Group is Empty: {x.Key}")
        );
    }
}