using Microsoft.EntityFrameworkCore;

namespace Platform.Web.Services;

public class DatabasePlatformRepository: IPlatformRepository
{
    private readonly PlatformDbContext _context;

    public DatabasePlatformRepository(PlatformDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Models.Platform>> GetAllPlatformsAsync(CancellationToken token = default)
    {
        return await _context.Platforms.ToListAsync(token);
    }

    public async Task<Models.Platform?> FindPlatformByIdAsync(int id, CancellationToken token = default)
    {
        return await _context.Platforms.SingleOrDefaultAsync(x => x.Id == id, token);
    }

    public async Task<Models.Platform> CreatePlatformAsync(string name, string publisher, string cost, CancellationToken token = default)
    {
        var platform = new Models.Platform()
        {
            Name = name,
            Cost = cost,
            Publisher = publisher
        };

        var entity = await _context.Platforms.AddAsync(platform, token);
        await _context.SaveChangesAsync(token);
        return entity.Entity;
    }
}