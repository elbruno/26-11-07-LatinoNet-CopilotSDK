// App web local: el backend mantiene la credencial y envia deltas por SSE.
using System.Threading.Channels;
using AppliedExamples;
using GitHub.Copilot;

var builder = WebApplication.CreateBuilder([]);
builder.WebHost.UseUrls("http://127.0.0.1:5057");
var app = builder.Build();

// El cliente vive tanto como el servidor, no en el navegador.
await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();

var page = await File.ReadAllTextAsync(
    Path.Combine(AppContext.BaseDirectory, "wwwroot", "index.html"));
app.MapGet("/", () => Results.Content(page, "text/html; charset=utf-8"));

app.MapPost("/chat", async (ChatRequest request, HttpContext context) =>
{
    if (string.IsNullOrWhiteSpace(request.Prompt))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(
            new { error = "Escribe una pregunta antes de enviar." },
            context.RequestAborted);
        return;
    }

    var cancellationToken = context.RequestAborted;
    await using var session = await client.CreateSessionAsync(new SessionConfig
    {
        Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
        Streaming = true,
        AvailableTools = [],
        EnableConfigDiscovery = false,
        EnableSkills = false,
        EnableHostGitOperations = false
    });

    var deltas = Channel.CreateUnbounded<string>();
    using var subscription = session.On<AssistantMessageDeltaEvent>(
        delta => deltas.Writer.TryWrite(delta.Data.DeltaContent));
    var logger = app.Services.GetRequiredService<ILoggerFactory>()
        .CreateLogger("AppliedExamples.Web");
    context.Response.ContentType = "text/event-stream";
    context.Response.Headers.CacheControl = "no-cache";
    context.Response.Headers.Append("X-Accel-Buffering", "no");

    var generation = GenerateAndCloseAsync();
    try
    {
        // El canal deja que cada fragmento llegue al navegador mientras se genera.
        await foreach (var delta in deltas.Reader.ReadAllAsync(cancellationToken))
        {
            await WriteEventAsync("delta", delta, cancellationToken);
        }

        await generation;
        await WriteEventAsync("done", "La respuesta termino.", cancellationToken);
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
        // El cliente cerro o cancelo la solicitud. AbortAsync detiene el turno actual.
        try
        {
            await generation;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }
    catch (Exception exception)
    {
        logger.LogError(exception, "The Copilot web turn failed.");
        await WriteEventAsync(
            "error",
            "La respuesta fallo. Revisa la salida del servidor.",
            cancellationToken);
    }
    finally
    {
        deltas.Writer.TryComplete();
    }

    async Task GenerateAndCloseAsync()
    {
        try
        {
            await WebTurnExecution.RunAsync(
                async token =>
                {
                    await session.SendAndWaitAsync(
                        new MessageOptions { Prompt = request.Prompt },
                        cancellationToken: token);
                },
                session.AbortAsync,
                cancellationToken);
        }
        finally
        {
            deltas.Writer.TryComplete();
        }
    }

    async Task WriteEventAsync(string eventName, string data, CancellationToken token)
    {
        await context.Response.WriteAsync(SseProtocol.Format(eventName, data), token);
        await context.Response.Body.FlushAsync(token);
    }
});

await app.RunAsync();

internal sealed record ChatRequest(string Prompt);
