using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace EFCoreExtensions.Queryable
{
    public static partial class EFCoreQueryableExtensions
    {
        private const string QueryTagHead = "<SqlTag>";
        private const string QueryTagTail = "</SqlTag>";
        private const string QueryTagFormat = QueryTagHead + "{0}" + QueryTagTail;
        private const string QueryTagRegexGroupName = "EFCoreQueryTag";

        private static readonly Regex QueryTagExtractor =
            new Regex($@"{QueryTagHead}(?<{QueryTagRegexGroupName}>\w+){QueryTagTail}");

        /// <summary>
        /// Add a query tag to the generated SQL. The tag recommends only contains letters, numbers, or underlines.
        /// </summary>
        /// <seealso href="https://docs.microsoft.com/en-us/ef/core/querying/tags">Query tags</seealso>
        public static IQueryable<T> QueryTagWith<T>(this IQueryable<T> queryable, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException($"Argument cannot be null or empty", $"{nameof(tag)}");
            }

            tag = string.Format(QueryTagFormat, tag);
            return queryable.TagWith(tag);
        }

        /// <summary>
        /// Add a tag for DELETE, INSERT and UPDATE operation. The tag recommends only contains letters, numbers, or underlines.
        /// </summary>
        public static DbContext NonQueryTagWith(this DbContext dbContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extract the first tag from SQL, if not matched, then empty string return.
        /// </summary>
        public static string ExtractFirstTag(this string sql)
        {
            var match = QueryTagExtractor.Match(sql);

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
    }
}