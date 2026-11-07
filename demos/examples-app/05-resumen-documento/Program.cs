// Resumen: lee un documento publico incluido junto al ejecutable, no desde cwd.
using AppliedExamples;
using GitHub.Copilot;

var documentPath = Path.Combine(
    AppContext.BaseDirectory, "Documents", "sample-workshop-brief.md");
var document = await File.ReadAllTextAsync(documentPath);

await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
    AvailableTools = [],
    EnableConfigDiscovery = false,
    EnableSkills = false,
    EnableHostGitOperations = false
});

var response = await session.SendAndWaitAsync(new MessageOptions
{
    Prompt = DocumentSummaryPrompt.Build(document)
});
Console.WriteLine(response?.Data.Content
    ?? throw new InvalidOperationException("El turno termino sin respuesta."));
