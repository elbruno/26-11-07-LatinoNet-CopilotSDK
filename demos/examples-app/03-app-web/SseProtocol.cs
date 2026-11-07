using System.Text.Json;

namespace AppliedExamples;

public static class SseProtocol
{
    public static string Format(string eventName, string data) =>
        $"event: {eventName}\ndata: {JsonSerializer.Serialize(data)}\n\n";
}
