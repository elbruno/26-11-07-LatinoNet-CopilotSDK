// Tool local: una funcion .NET de solo lectura, sin parametros ni datos privados.
using GitHub.Copilot;
using Microsoft.Extensions.AI;

// El host posee el dato. El modelo solo puede solicitar esta funcion.
var tool = CopilotTool.DefineTool(
    () =>
    {
        Console.WriteLine("[handler] sdk_concept_lookup");
        return "Streaming entrega fragmentos de una respuesta mientras se genera.";
    },
    // Omitir permiso adicional SOLO para esta lectura interna y acotada.
    toolOptions: new CopilotToolOptions { SkipPermission = true },
    factoryOptions: new AIFunctionFactoryOptions
    {
        Name = "sdk_concept_lookup",
        Description = "Devuelve la definicion aprobada de streaming."
    });

await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
    Tools = [tool], // Registrar el handler no es lo mismo que exponer su nombre.
    AvailableTools = ["sdk_concept_lookup"],
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
