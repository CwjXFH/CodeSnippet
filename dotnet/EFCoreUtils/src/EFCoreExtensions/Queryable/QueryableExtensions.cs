using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EFCoreExtensions.Queryable
{
    public static partial class EFCoreQueryableExtensions
    {
        /// <summary>
        /// Asynchronously creates a <see cref="List{T}" /> from an <see cref="IQueryable{T}" /> by enumerating it asynchronously.
        /// </summary>
        /// <param name="dataCount">
        /// The number of result set data, which is used to pre-allocate memory to avoid performance and memory
        /// consumption caused by <see cref="List{T}"/> dynamic expansion during the query process.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source, int dataCount,
            CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"{nameof(source)}");
            }

            if (source is IAsyncEnumerable<TSource> asyncSource == false)
            {
                throw new InvalidOperationException($"Parameter is not {typeof(IAsyncEnumerable<>)} type.");
            }

            var list = new List<TSource>(dataCount);
            await foreach (var element in asyncSource.WithCancellation(cancellationToken))
            {
                list.Add(element);
            }

            return list;
        }
    }
}
