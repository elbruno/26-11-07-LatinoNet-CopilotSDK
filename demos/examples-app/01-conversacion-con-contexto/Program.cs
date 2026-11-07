// Conversacion: tres turnos dentro de una misma sesion conservan el contexto.
using AppliedExamples;
using GitHub.Copilot;

await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();

// La sesion se crea una sola vez; cada prompt agrega un nuevo turno a ella.
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
    AvailableTools = [],
    EnableConfigDiscovery = false,
    EnableSkills = false,
    EnableHostGitOperations = false
});

var turn = 0;
await ConversationSteps.RunAsync(async prompt =>
{
    turn++;
    Console.WriteLine($"\n--- Turno {turn} ---");
    Console.WriteLine($"Pregunta: {prompt}");

    var response = await session.SendAndWaitAsync(new MessageOptions { Prompt = prompt });
    var answer = response?.Data.Content
        ?? throw new InvalidOperationException("El turno termino sin respuesta.");
    Console.WriteLine($"Respuesta: {answer}");
    return answer;
});
