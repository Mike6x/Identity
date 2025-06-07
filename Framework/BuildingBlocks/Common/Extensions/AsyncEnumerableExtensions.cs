namespace BuildingBlocks.Common.Extensions;

/// <summary>
/// https://stackoverflow.com/questions/59380470/convert-iasyncenumerable-to-list
/// </summary>
public static class AsyncEnumerableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source,
        CancellationToken cancellationToken = default)
    {
        var list = new List<T>();
        await foreach (var item in source.WithCancellation(cancellationToken)
                           .ConfigureAwait(false))
            list.Add(item);
        return list;
    }
}