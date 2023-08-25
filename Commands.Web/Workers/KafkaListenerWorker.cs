using Bogus;
using Commands.Web.Services;
using Common.KafkaEvents;
using Confluent.Kafka;

namespace Commands.Web.Workers;

public class KafkaListenerWorker: BackgroundService
{
    private readonly ILogger<KafkaListenerWorker> _logger;
    private readonly ICommandRepository _repo;
    private readonly IConfiguration _configuration;

    public KafkaListenerWorker(ILogger<KafkaListenerWorker> logger, ICommandRepository repo, IConfiguration configuration)
    {
        _logger = logger;
        _repo = repo;
        _configuration = configuration;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        _logger.LogInformation("Создаю консьюмера");
        var consumer = CreateConsumer();
        _logger.LogInformation("Консьюмер создан. Подписываюсь на топик");
        consumer.Subscribe("test");
        _logger.LogInformation("Начинаю читать сообщения");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var e = Helpers.Deserialize<PlatformCreatedEvent>(result.Message.Value);
                await ProcessAsync(e, stoppingToken);

            }
            catch (OperationCanceledException)
            { }
        }
        _logger.LogInformation("Закрываю консьюмера");
        consumer.Close();
    }

    private static readonly Faker Faker = new("ru");
    
    private async Task ProcessAsync(PlatformCreatedEvent e, CancellationToken token)
    {
        var commands = Faker.Random.WordsArray(1, 3);
        var description = Faker.Random.Words(10);
        _logger.LogInformation("Сохраняю команду в БД");
        await _repo.CreateCommandAsync(e.PlatformId, e.Name, commands, description, token);
        _logger.LogInformation("Команда сохранена в БД");
    }

    private IConsumer<Null, byte[]> CreateConsumer()
    {
        return new ConsumerBuilder<Null, byte[]>(new ConsumerConfig()
            {
                BootstrapServers = _configuration.GetValue<string>("KAFKA_BOOTSTRAP_SERVERS"),
                GroupId = "Commands.Web"
            })
           .Build();
    }
}