using DatabaseObserver.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

var configuration = new ConfigurationBuilder()
                   .AddEnvironmentVariables()
                   .Build();

var loggerFactory = new LoggerFactory();
var logger = loggerFactory.CreateLogger("Main");

logger.LogInformation("Создаю источник подключения к БД");
await using var builder = new NpgsqlDataSourceBuilder(configuration.GetValue<string>("DATABASE_CONNECTION_STRING"))
                         .UseLoggerFactory(loggerFactory)
                         .Build();

try
{
    await ScanDatabaseAsync(builder, cts.Token);
    logger.LogInformation("Работа приложения завершена");
}
catch (OperationCanceledException) when (cts.IsCancellationRequested)
{
    logger.LogWarning("Превышено максимальное время работы приложения");
}
catch (Exception e)
{
    logger.LogCritical(e, "Ошибка во время исполнения программы");
}

async Task ScanDatabaseAsync(NpgsqlDataSource source, CancellationToken stoppingToken)
{
    var platforms = await ReadAllPlatformsAsync(source, stoppingToken);
    LogPlatforms(platforms);
}

void LogPlatforms(List<Platform> platforms)
{
    logger.LogInformation("Всего платформ: {Count}. Платформы: {@Platforms}", platforms.Count, platforms);
}


async Task<List<Platform>> ReadAllPlatformsAsync(NpgsqlDataSource source, CancellationToken stoppingToken)
{
    var result = new List<Platform>();
    await using var command =
        source.CreateCommand(@"SELECT ""Id"", ""Name"", ""Cost"", ""Publisher"" FROM ""Platforms""");
    logger.LogInformation("Отправляю команду в БД");
    var reader = await command.ExecuteReaderAsync(stoppingToken);
    logger.LogInformation("Команда отправлена. Начинаю читать записи");
    while (await reader.ReadAsync(stoppingToken))
    {
        var platform = GetPlatform();
        result.Add(platform);
    }
    logger.LogInformation("Все записи прочитаны");
    return result;

    Platform GetPlatform()
    {
        var id = reader!.GetInt32(0);
        var name = reader.GetString(1);
        var cost = reader.GetString(2);
        var publisher = reader.GetString(3);
        return new Platform() {Cost = cost, Id = id, Name = name, Publisher = publisher};
    }
}