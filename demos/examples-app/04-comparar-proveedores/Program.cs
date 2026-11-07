// Comparacion ilustrativa: el mismo prompt con runtime Copilot y BYOK Foundry.
using Azure.Core;
using Azure.Identity;
using GitHub.Copilot;
using Microsoft.Extensions.Configuration;

const string prompt = "Explica en dos frases que gana una aplicacion al transmitir una respuesta por streaming.";
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["Foundry:BaseUrl"]
    ?? throw new InvalidOperationException("Configura Foundry con Set-DemoSecrets.ps1.");
var model = config["Foundry:Model"]
    ?? throw new InvalidOperationException("Falta Foundry:Model.");
var provider = new ProviderConfig
{
    Type = "openai",
    BaseUrl = endpoint,
    WireApi = "responses",
    ModelId = model,
    WireModel = model
};

// Se reutiliza el mismo UserSecretsId y autenticacion que BYOK 01.
var key = config["Foundry:ApiKey"];
if (!string.IsNullOrWhiteSpace(key))
{
    provider.ApiKey = key;
}
else
{
    var credential = new AzureCliCredential();
#pragma warning disable GHCP001
    provider.BearerTokenProvider = async _ => (await credential.GetTokenAsync(
        new TokenRequestContext(["https://ai.azure.com/.default"]))).Token;
#pragma warning restore GHCP001
}

await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();
await using var copilotSession = await client.CreateSessionAsync(new SessionConfig
{
    Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
    AvailableTools = [],
    EnableConfigDiscovery = false,
    EnableSkills = false,
    EnableHostGitOperations = false
});
await using var foundrySession = await client.CreateSessionAsync(new SessionConfig
{
    Model = model,
    Provider = provider,
    AvailableTools = [],
    EnableConfigDiscovery = false,
    EnableSkills = false,
    EnableHostGitOperations = false
});

Console.WriteLine($"Pregunta comun: {prompt}");
Console.WriteLine("\n--- Runtime Copilot ---");
var copilot = await copilotSession.SendAndWaitAsync(new MessageOptions { Prompt = prompt });
Console.WriteLine(copilot?.Data.Content
    ?? throw new InvalidOperationException("Copilot termino sin respuesta."));

Console.WriteLine($"\n--- Microsoft Foundry BYOK ({model}) ---");
var foundry = await foundrySession.SendAndWaitAsync(new MessageOptions { Prompt = prompt });
Console.WriteLine(foundry?.Data.Content
    ?? throw new InvalidOperationException("Foundry termino sin respuesta."));
Console.WriteLine("\nComparacion ilustrativa, no es un benchmark de calidad o velocidad.");
