using Commands.Web.Models;

namespace Commands.Web.Services;

public interface ICommandRepository
{
    public Task<Command> CreateCommandAsync(int platformId, string platformName, string[] commands, string description, CancellationToken token = default);
    public Task<IEnumerable<Command>> GetAllCommandsAsync(CancellationToken token = default);
}