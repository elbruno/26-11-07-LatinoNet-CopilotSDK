// BYOK 1: Foundry cloud. Entra por defecto; API key opcional del mismo recurso.
using Azure.Core;
using Azure.Identity;
using GitHub.Copilot;
using Microsoft.Extensions.Configuration;

// Set-DemoSecrets.ps1 guarda configuracion fuera del repo; nunca mostrarla.
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["Foundry:BaseUrl"]
    ?? throw new InvalidOperationException("Configura Foundry:BaseUrl con Set-DemoSecrets.ps1.");
var model = config["Foundry:Model"]
    ?? throw new InvalidOperationException("Falta Foundry:Model.");
var provider = new ProviderConfig
{
    Type = "openai",
    BaseUrl = endpoint, // Endpoint de recurso /openai/v1/, no de proyecto.
    WireApi = "responses",
    ModelId = model,
    WireModel = model
};

// Absorbe la variante byok-simple sin otra consola ni selector por argumentos.
var key = config["Foundry:ApiKey"];
if (!string.IsNullOrWhiteSpace(key))
{
    provider.ApiKey = key;
}
else
{
    var credential = new AzureCliCredential(); // Login ya realizado, no inicia uno.
#pragma warning disable GHCP001
    provider.BearerTokenProvider = async _ => (await credential.GetTokenAsync(
        new TokenRequestContext(["https://ai.azure.com/.default"]))).Token;
#pragma warning restore GHCP001
}

await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot"),
    UseLoggedInUser = false // BYOK no necesita la identidad de GitHub Copilot.
});
await client.StartAsync();
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = model,
    Provider = provider,
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
