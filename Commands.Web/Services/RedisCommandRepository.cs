using System.Text.Json;
using Commands.Web.Models;
using StackExchange.Redis;

namespace Commands.Web.Services;

public class RedisCommandRepository: ICommandRepository
{
    private readonly IConnectionMultiplexer _connection;
    private readonly ILogger<RedisCommandRepository> _logger;

    public RedisCommandRepository(IConnectionMultiplexer connection, ILogger<RedisCommandRepository> logger)
    {
        _connection = connection;
        _logger = logger;
    }
    
    public async Task<Command> CreateCommandAsync(int platformId,
                                                  string platformName,
                                                  string[] commands,
                                                  string description,
                                                  CancellationToken token = default)
    {
        var command = new Command()
        {
            Id = Guid.NewGuid(),
            Commands = commands,
            PlatformId = platformId,
            Description = description,
            PlatformName = platformName
        };
        
        var serialized = JsonSerializer.Serialize(command);
        _logger.LogInformation("Получаю базу данных");
        var db = _connection.GetDatabase();
        var platformIdString = platformId.ToString();
        _logger.LogInformation("Создаю батч");
        await db.SetAddAsync("platforms", platformIdString);
        await db.SetAddAsync(new RedisKey($"commands-{platformIdString}"), new RedisValue(serialized));
        return command;
    }

    public async Task<IEnumerable<Command>> GetAllCommandsAsync(CancellationToken token = default)
    {
        var db = _connection.GetDatabase();
        var platformIds = await GetAllPlatformIdsAsync(db);
        var result = new List<Command[]>(platformIds.Length);
        foreach (var id in platformIds)
        {
            result.Add(await GetCommandsForPlatform(id, db));
        }

        return result.SelectMany(v => v);
    }

    private async Task<Command[]> GetCommandsForPlatform(string platformId, IDatabase database)
    {
        var key = $"commands-{platformId}";
        var result = await database.SetMembersAsync(key);
        return Array.ConvertAll(result, v => JsonSerializer.Deserialize<Command>(v.ToString()) ?? throw new ArgumentNullException($"Команда {v.ToString()} для платформы {platformId} десериализовалась в null"));
    }

    private async Task<string[]> GetAllPlatformIdsAsync(IDatabase database)
    {
        var values = await database.SetMembersAsync("platforms");
        return Array.ConvertAll(values, v => v.ToString());
    }
}