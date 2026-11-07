// Skills: instrucciones reutilizables, copiadas junto al ejecutable al compilar.
using GitHub.Copilot;
using GitHub.Copilot.Rpc;

await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
    EnableSkills = true,
    SkillDirectories = [Path.Combine(AppContext.BaseDirectory, "Skills")],
    // No depende del directorio desde donde se lance dotnet run.
    // Mantener la politica de rechazo del ejemplo original al cargar la skill.
#pragma warning disable GHCP001
    OnPermissionRequest = (_, _) => Task.FromResult(
        PermissionDecision.Reject("Esta demo no autoriza tools externas.")),
#pragma warning restore GHCP001
    EnableConfigDiscovery = false,
    EnableHostGitOperations = false
});
var response = await session.SendAndWaitAsync(new MessageOptions
{
    Prompt = "Explica como se combinan host, sesion, prompt y eventos en Copilot SDK."
});
Console.WriteLine(response?.Data.Content
    ?? throw new InvalidOperationException("El turno termino sin respuesta."));
