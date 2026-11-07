// Streaming: observar deltas sin escribir un runner de eventos o de etapas.
using GitHub.Copilot;

await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
    Streaming = true, // El runtime publica fragmentos mientras genera.
    AvailableTools = [],
    EnableConfigDiscovery = false,
    EnableSkills = false,
    EnableHostGitOperations = false
});

// Suscribirse ANTES de enviar; Dispose elimina el handler al salir.
using var subscription = session.On<AssistantMessageDeltaEvent>(
    delta => Console.Write(delta.Data.DeltaContent));

// SendAndWaitAsync espera el turno completo y propaga los errores del SDK.
await session.SendAndWaitAsync(new MessageOptions
{
    Prompt = "Explica en espanol, en tres frases, que aporta streaming a una app."
});
Console.WriteLine();
