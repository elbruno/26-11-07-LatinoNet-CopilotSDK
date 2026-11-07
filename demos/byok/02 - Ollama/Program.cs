// BYOK 2: Ollama local mediante Chat Completions, sin clave ni descarga de modelos.
using GitHub.Copilot;

var endpoint = Environment.GetEnvironmentVariable("OLLAMA_BASE_URL") ?? throw new InvalidOperationException("Configura el endpoint local segun demos/byok/README.md.");

var model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? throw new InvalidOperationException("Falta OLLAMA_MODEL.");

// muestra en la consola el endpoint y el modelo configurados.
Console.WriteLine($"Endpoint: {endpoint}");
Console.WriteLine($"Modelo: {model}");

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
        Type = "openai", // Describe el protocolo, NO el servicio de OpenAI.
        BaseUrl = endpoint,
        WireApi = "completions",
        ModelId = model,
        WireModel = model
        // Copilot SDK permite omitir ApiKey para Ollama.
    },
    AvailableTools = [], // La demo no ejecuta tools, ni shell ni Git.
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
