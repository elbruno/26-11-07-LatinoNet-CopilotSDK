// BYOK 3: Foundry Local ya iniciado. El runbook descubre el puerto, no lo inventa.
using GitHub.Copilot;

var endpoint = Environment.GetEnvironmentVariable("FOUNDRY_LOCAL_BASE_URL") ?? throw new InvalidOperationException("Configura el endpoint real segun demos/byok/README.md.");
var model = Environment.GetEnvironmentVariable("FOUNDRY_LOCAL_MODEL") ?? throw new InvalidOperationException("Falta FOUNDRY_LOCAL_MODEL.");

await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot"),
    UseLoggedInUser = false
});
await client.StartAsync();
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = model,
    Provider = new ProviderConfig
    {
        Type = "openai",
        BaseUrl = endpoint, // /v1/ en el puerto dinamico del daemon existente.
        WireApi = "completions",
        ModelId = model,
        WireModel = model
        // Foundry Local no requiere una API key.
    },
    AvailableTools = [],
    EnableConfigDiscovery = false,
    EnableSkills = false,
    EnableHostGitOperations = false
});
var response = await session.SendAndWaitAsync(new MessageOptions
{
    Prompt = "Explica BYOK en espanol en una frase."
});
Console.WriteLine(response?.Data.Content
    ?? throw new InvalidOperationException("El turno termino sin respuesta."));
