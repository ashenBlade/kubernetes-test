using System.Text.Json;

namespace Common.KafkaEvents;

public static class Helpers
{
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    public static byte[] Serialize<T>(T e)
    {
        return JsonSerializer.SerializeToUtf8Bytes(e, Options);
    }

    public static T Deserialize<T>(byte[] data) => JsonSerializer.Deserialize<T>(data, Options)!;
}