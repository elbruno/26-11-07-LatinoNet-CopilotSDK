// Hola mundo: cliente -> sesion -> prompt -> respuesta. Sin argumentos ni modos.
using GitHub.Copilot;

// 1. Iniciar el runtime instalado; await using libera sus recursos al terminar.
await using var client = new CopilotClient(new CopilotClientOptions
{
    Connection = RuntimeConnection.ForStdio("copilot")
});
await client.StartAsync();

// 2. Crear una conversacion sin tools, skills, discovery ni operaciones Git.
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = Environment.GetEnvironmentVariable("COPILOT_DEMO_MODEL"),
    AvailableTools = [],
    EnableConfigDiscovery = false,
    EnableSkills = false,
    EnableHostGitOperations = false
});

// 3. Enviar una sola pregunta; el SDK espera el final del turno.
var response = await session.SendAndWaitAsync(new MessageOptions
{
    Prompt = "Explica en una frase en espanol para que sirve Microsoft Copilot SDK."
});
Console.WriteLine(response?.Data.Content
    ?? throw new InvalidOperationException("El turno termino sin respuesta."));
