using Microsoft.EntityFrameworkCore;

namespace Platform.Web;

public class PlatformDbContext: DbContext
{
    public DbSet<Models.Platform> Platforms => Set<Models.Platform>();

    public PlatformDbContext(DbContextOptions<PlatformDbContext> options)
        : base(options)
    { }
}