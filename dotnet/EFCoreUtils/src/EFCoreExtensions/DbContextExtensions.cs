using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EFCoreExtensions
{
    public static class DbContextExtensions
    {
        public static async Task<int> SaveChangeAsync(this DbContext dbContext, string sqlTag, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sqlTag))
            {
                throw new ArgumentNullException($"{nameof(sqlTag)}");
            }
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
