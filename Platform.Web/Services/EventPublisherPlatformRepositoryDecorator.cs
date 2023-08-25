using Common.KafkaEvents;
using Confluent.Kafka;

namespace Platform.Web.Services;

public class EventPublisherPlatformRepositoryDecorator: IPlatformRepository
{
    private readonly IProducer<Null, byte[]> _producer;
    private IPlatformRepository _repo;

    public EventPublisherPlatformRepositoryDecorator(IProducer<Null, byte[]> producer, IPlatformRepository repo)
    {
        _producer = producer;
        _repo = repo;
    }

    public async Task<IReadOnlyList<Models.Platform>> GetAllPlatformsAsync(CancellationToken token = default)
    {
        return await _repo.GetAllPlatformsAsync(token);
    }

    public async Task<Models.Platform?> FindPlatformByIdAsync(int id, CancellationToken token = default)
    {
        return await _repo.FindPlatformByIdAsync(id, token);
    }

    public async Task<Models.Platform> CreatePlatformAsync(string name, string publisher, string cost, CancellationToken token = default)
    {
        var platform = await _repo.CreatePlatformAsync(name, publisher, cost, token);
        
        await _producer.ProduceAsync("test",
            new Message<Null, byte[]>()
            {
                Value = Common.KafkaEvents.Helpers.Serialize(new PlatformCreatedEvent(platform.Id, platform.Name,
                    platform.Cost, platform.Publisher))
            }, token);
        
        return platform;
    }
}