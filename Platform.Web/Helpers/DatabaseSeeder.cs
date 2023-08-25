using Microsoft.EntityFrameworkCore;

namespace Platform.Web.Helpers;

public static class DatabaseSeeder
{
    public static async Task SeedDbAsync(WebApplication builder)
    {
        await using var sp = builder.Services.CreateAsyncScope();
        await SeedAsync(sp.ServiceProvider.GetRequiredService<PlatformDbContext>());
    }


    private static async Task SeedAsync(PlatformDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        if (!await context.Platforms.AnyAsync())
        {
            await context.Platforms.AddRangeAsync(new[]
            {
                new Models.Platform()
                {
                    Name = ".NET", Publisher = "Microsoft", Cost = "free",
                },
                new Models.Platform()
                {
                    Name = "AWS", Publisher = "Amazon", Cost = "many money"
                }
            });

            await context.SaveChangesAsync();
        }
    }
}