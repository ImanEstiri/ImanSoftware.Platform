// Core/Extensions/EnumerableExtensions.cs
namespace ImanSoftware.Framework.Core.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Checks if an enumerable is null or empty.
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        => source is null || !source.Any();

    /// <summary>
    /// Converts an enumerable to a HashSet.
    /// </summary>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        => new(source);

    /// <summary>
    /// Performs an action for each item in an enumerable.
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }

    /// <summary>
    /// Returns distinct elements based on a specified key selector.
    /// </summary>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
                yield return element;
        }
    }

    /// <summary>
    /// Splits an enumerable into chunks of a specified size.
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
    {
        if (chunkSize <= 0)
            throw new ArgumentException("Chunk size must be positive.", nameof(chunkSize));

        var chunk = new List<T>(chunkSize);
        foreach (var item in source)
        {
            chunk.Add(item);
            if (chunk.Count == chunkSize)
            {
                yield return chunk;
                chunk = new List<T>(chunkSize);
            }
        }
        if (chunk.Any())
            yield return chunk;
    }
}