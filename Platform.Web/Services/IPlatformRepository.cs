namespace Platform.Web.Services;

public interface IPlatformRepository
{
    Task<IReadOnlyList<Models.Platform>> GetAllPlatformsAsync(CancellationToken token = default);

    Task<Models.Platform?> FindPlatformByIdAsync(int id, CancellationToken token = default);

    Task<Models.Platform> CreatePlatformAsync(string name, string publisher, string cost, CancellationToken token = default);
}