// BYOK 4: OpenAI directo con su API key, no una clave o suscripcion de Azure.
using GitHub.Copilot;
using Microsoft.Extensions.Configuration;

// User Secrets, no claves en codigo, argumentos ni variables persistentes.
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var model = config["OpenAI:Model"]
    ?? throw new InvalidOperationException("Ejecuta Set-DemoSecrets.ps1 -Provider OpenAI.");
var key = config["OpenAI:ApiKey"]
    ?? throw new InvalidOperationException("Falta OpenAI:ApiKey.");

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
        BaseUrl = "https://api.openai.com/v1/",
        WireApi = "responses",
        ApiKey = key, // Nunca imprimirla; solo el runtime la recibe.
        ModelId = model,
        WireModel = model
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
