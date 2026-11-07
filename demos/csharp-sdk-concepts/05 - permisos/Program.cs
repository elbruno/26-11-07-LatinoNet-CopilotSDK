// Permisos: aprobar UNA invocacion de una tool conocida, no aprobar todo.
using GitHub.Copilot;
using GitHub.Copilot.Rpc;
using Microsoft.Extensions.AI;

var tool = CopilotTool.DefineTool(
    () =>
    {
        Console.WriteLine("[handler] sdk_concept_lookup");
        return "El host autoriza operaciones; el prompt no concede permisos.";
    },
    toolOptions: new CopilotToolOptions { SkipPermission = false },
    factoryOptions: new AIFunctionFactoryOptions
    {
        Name = "sdk_concept_lookup",
        Description = "Devuelve la definicion aprobada de permisos."
    });

await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
    Tools = [tool],
    AvailableTools = ["sdk_concept_lookup"],
    // Esta politica ES la capacidad que mostramos, no un framework de checks.
#pragma warning disable GHCP001
    OnPermissionRequest = (request, _) =>
    {
        var known = request is PermissionRequestCustomTool { ToolName: "sdk_concept_lookup" };
        Console.WriteLine(known ? "[permission] allow-once" : "[permission] deny");
        return Task.FromResult(known
            ? PermissionDecision.ApproveOnce()
            : PermissionDecision.Reject("Solo se autoriza sdk_concept_lookup."));
    },
#pragma warning restore GHCP001
    EnableConfigDiscovery = false,
    EnableSkills = false,
    EnableHostGitOperations = false
});
var response = await session.SendAndWaitAsync(new MessageOptions
{
    Prompt = "Invoca sdk_concept_lookup y explica su resultado en espanol."
});
Console.WriteLine(response?.Data.Content
    ?? throw new InvalidOperationException("El turno termino sin respuesta."));
