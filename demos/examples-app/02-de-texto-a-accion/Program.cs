// Texto a accion: el SDK puede solicitar una busqueda local acotada.
using AppliedExamples;
using GitHub.Copilot;
using GitHub.Copilot.Rpc;
using Microsoft.Extensions.AI;

var lookup = CopilotTool.DefineTool(
    SupportTicketCatalog.Lookup,
    toolOptions: new CopilotToolOptions { SkipPermission = false },
    factoryOptions: new AIFunctionFactoryOptions
    {
        Name = SupportTicketCatalog.ToolName,
        Description = "Busca un ticket sintetico por su ID y devuelve solo el resumen local."
    });

await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
    Tools = [lookup],
    AvailableTools = [SupportTicketCatalog.ToolName],
#pragma warning disable GHCP001
    // Solo se aprueba una invocacion de la tool conocida; todo lo demas se rechaza.
    OnPermissionRequest = (request, _) => Task.FromResult(
        request is PermissionRequestCustomTool custom &&
        SupportTicketCatalog.AllowsTool(custom.ToolName)
            ? PermissionDecision.ApproveOnce()
            : PermissionDecision.Reject("Solo se autoriza la consulta local de tickets.")),
#pragma warning restore GHCP001
    EnableConfigDiscovery = false,
    EnableSkills = false,
    EnableHostGitOperations = false
});

var response = await session.SendAndWaitAsync(new MessageOptions
{
    Prompt = "Consulta LAT-1042. Resume el problema y recomienda una siguiente accion prudente usando solo el resumen del ticket."
});
Console.WriteLine(response?.Data.Content
    ?? throw new InvalidOperationException("El turno termino sin respuesta."));
