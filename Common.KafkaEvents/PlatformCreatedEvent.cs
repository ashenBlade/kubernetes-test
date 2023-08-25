namespace Common.KafkaEvents;

public record PlatformCreatedEvent(int PlatformId, string Name, string Cost, string Publisher);