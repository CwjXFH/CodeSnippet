using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EFCoreExtensions.Queryable
{
    public static partial class EFCoreQueryableExtensions
    {
        private const string QueryTagHead = "<SqlTag>";
        private const string QueryTagTail = "</SqlTag>";
        private const string QueryTagFormat = QueryTagHead + "{0}" + QueryTagTail;
        private const string QueryTagRegexGroupName = "EFCoreSQLTag";

        private static readonly Regex QueryTagExtractor =
            new Regex($@"{QueryTagHead}(?<{QueryTagRegexGroupName}>\w+){QueryTagTail}");

        private static readonly MethodInfo NonQueryTagWithMethodInfo = typeof(EFCoreQueryableExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Single(mi => mi.Name == nameof(NonQueryTagWith) && mi.IsGenericMethod && mi.GetParameters().Select(p => p.ParameterType)
                              .SequenceEqual(new[] { typeof(IQueryable<>).MakeGenericType(mi.GetGenericArguments()), typeof(string) }));

        /// <summary>
        /// Add a query tag to the generated SQL. The tag recommends only contains letters, numbers, or underlines.
        /// </summary>
        /// <seealso href="https://docs.microsoft.com/en-us/ef/core/querying/tags">Query tags</seealso>
        public static IQueryable<T> QueryTagWith<T>(this IQueryable<T> queryable, string tag)
        {
            if (queryable is not EntityQueryProvider)
            {
#pragma warning disable EF1001 // Internal EF Core API usage.
                throw new ArgumentException($"Parameter must be {typeof(EntityQueryProvider)} type.", nameof(queryable));
#pragma warning restore EF1001 // Internal EF Core API usage.
            }
            tag = FormatTag(tag);
            return queryable.TagWith(tag);
        }

        /// <summary>
        /// Add a tag for DELETE, INSERT and UPDATE operation. The tag recommends only contains letters, numbers, or underlines.
        /// </summary>
        public static DbSet<T> NonQueryTagWith<T>(this DbSet<T> dbSet, string tag)
            where T : class
        {

#pragma warning disable S125 // Sections of code should not be commented out
            // tag = FormatTag(tag);
            // var queryable = (IQueryable<T>)dbSet;

            // Expression.Call(dbContext)
            throw new NotImplementedException();
#pragma warning restore S125 // Sections of code should not be commented out
        }

        public static IQueryable<T> NonQueryTagWith<T>(this IQueryable<T> queryable, string tag)
        {
            if (queryable is not EntityQueryProvider)
            {
#pragma warning disable EF1001 // Internal EF Core API usage.
                throw new ArgumentException($"Parameter must be {typeof(EntityQueryProvider)} type.", nameof(queryable));
#pragma warning restore EF1001 // Internal EF Core API usage.
            }

            tag = FormatTag(tag);

            MethodCallExpression methodCallExpression = Expression.Call((Expression)null!, NonQueryTagWithMethodInfo.MakeGenericMethod(typeof(T)),
                queryable.Expression, new NonQueryTagExpression(tag));
            return queryable.Provider.CreateQuery<T>((Expression)methodCallExpression);
        }

        /// <summary>
        /// Extract the first tag from SQL, if not matched, then empty string return.
        /// </summary>
        public static string ExtractFirstTag(this string sql)
        {
#pragma warning disable S1481 // Unused local variables should be removed
            var match = QueryTagExtractor.Match(sql);
#pragma warning restore S1481 // Unused local variables should be removed

#if NET6_0
            return match.Groups.TryGetValue(QueryTagRegexGroupName, out var group) ? group.Value ?? "" : "";
#endif

            throw new NotImplementedException();
        }

        /// <summary>
        /// Extract all tags from sql, if not matched, then return empty list.
        /// </summary>
        public static IReadOnlyCollection<string> ExtractAllTags(this string sql)
        {
            throw new NotImplementedException();
        }


        private static string FormatTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException($"{nameof(tag)}");
            }

            return string.Format(QueryTagFormat, tag);
        }
    }
}
