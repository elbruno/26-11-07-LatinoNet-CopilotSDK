// WorkIQ: un turno fijo, solo conteos agregados, sin chat ni opciones de entrada.
using CsharpSdkConcepts;
using GitHub.Copilot;
using GitHub.Copilot.Rpc;

const string question = "Cuantas reuniones tengo manana y cuantas se solapan?";
// Esta demo toca datos privados. El contrato de privacidad queda en su archivo,
// no se elimina para reducir lineas ni se imprime el JSON antes de comprobarlo.
WorkIqSafety.EnsureAggregateQuestion(question);
await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
    PluginDirectories = [WorkIqSafety.GetPluginDirectory()],
    EnableSkills = true,
    AvailableTools = ["workiq-ask"], // No exponer operaciones de escritura.
    Streaming = false, // No filtrar fragmentos de contenido privado a pantalla.
    SystemMessage = new SystemMessageConfig
    {
        Mode = SystemMessageMode.Replace,
        Content = WorkIqSafety.SystemMessage
    },
#pragma warning disable GHCP001
    OnPermissionRequest = (request, _) => Task.FromResult(
        request is PermissionRequestMcp { ServerName: "workiq" } mcp &&
        mcp.ToolName is "ask" or "workiq-ask"
            ? PermissionDecision.ApproveOnce()
            : PermissionDecision.Reject("Solo se autoriza workiq-ask.")),
#pragma warning restore GHCP001
    EnableConfigDiscovery = false,
    EnableHostGitOperations = false
});
// Si la tool falla, no aceptar una respuesta posterior inventada por el modelo.
var toolFailed = false;
var toolSucceeded = false;
using var toolErrors = session.On<ToolExecutionCompleteEvent>(tool =>
{
    if (!tool.Data.Success) toolFailed = true;
    else toolSucceeded = true;
});
var response = await session.SendAndWaitAsync(new MessageOptions
{
    Prompt = WorkIqSafety.BuildPrompt(question)
});
if (toolFailed || !toolSucceeded)
    throw new InvalidOperationException("No se completo WorkIQ; salida bloqueada.");
WorkIqSafety.Print(WorkIqSafety.ParseAndValidate(response?.Data.Content
    ?? throw new InvalidOperationException("El turno termino sin respuesta.")));
