using Microsoft.EntityFrameworkCore;

namespace Api.Database;

public class InfoDbContext(DbContextOptions<InfoDbContext> options) : DbContext(options)
{
    public DbSet<InfoEntity> Infos { set; get; }
}
