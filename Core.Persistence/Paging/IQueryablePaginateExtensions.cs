using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Core.Persistence.Paging;

public static class IQueryablePaginateExtensions
{
    /// <summary>
    /// Asynchronously paginates the <typeparamref name="T"/> and returns the paginated items.
    /// </summary>
    /// <typeparam name="T">Represents the object type to be paginated</typeparam>
    /// <param name="index">Represents the index of page the data to be fetched from. Starts from 0.</param>
    /// <param name="size">Represents the size of the page size of the fetched data.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the list of paginated items of <typeparamref name="T"/>.</returns>
    public static async Task<Paginate<T>> PaginateAsync<T>
        (
            this IQueryable<T> source,
            int index,
            int size,
            CancellationToken cancellationToken
        )
    {
        int count = await source.CountAsync(cancellationToken)
                                .ConfigureAwait(false);

        List<T> items = await source.Skip(index * size)
                                    .Take(size)
                                    .ToListAsync(cancellationToken)
                                    .ConfigureAwait(false);

        Paginate<T> paginatedData = new()
        {
            Index = index,
            Size = size,
            Count = count,
            Items = items,
            Pages = (int)Math.Ceiling(count / (double)size)
        };
        return paginatedData;
    }

    /// <summary>
    /// Paginates the <typeparamref name="T"/> and returns the paginated items.
    /// </summary>
    /// <typeparam name="T">Represents the object type to be paginated</typeparam>
    /// <param name="index">Represents the index of page the data to be fetched from. Starts from 0.</param>
    /// <param name="size">Represents the size of the page size of the fetched data.</param>
    /// <returns>The list of paginated items of <typeparamref name="T"/>.</returns>
    public static Paginate<T> Paginate<T>
        (
            this IQueryable<T> source,
            int index,
            int size
        )
    {
        int count = source.Count();


        List<T> items = source.Skip(index * size).Take(size).ToList();

        Paginate<T> paginatedData = new()
        {
            Index = index,
            Size = size,
            Count = count,
            Items = items,
            Pages = (int)Math.Ceiling(count / (double)size)
        };
        return paginatedData;
    }
}
