namespace AppliedExamples;

public static class ConversationSteps
{
    public static IReadOnlyList<string> Prompts { get; } =
    [
        "Ayudame a preparar un taller remoto de 60 minutos para desarrolladores .NET sobre Microsoft Copilot SDK. Propone objetivos y una agenda con demos.",
        "Reduce las demos a 15 minutos. Conserva el publico .NET, el tema y los 60 minutos totales; reserva 15 minutos para preguntas.",
        "Convierte la agenda revisada a formato de sesion remota, con transiciones y tiempos. Conserva los 15 minutos de demos y los 15 de preguntas."
    ];

    public static async Task<IReadOnlyList<string>> RunAsync(
        Func<string, Task<string>> sendAsync)
    {
        var answers = new List<string>(Prompts.Count);
        foreach (var prompt in Prompts)
        {
            answers.Add(await sendAsync(prompt));
        }

        return answers;
    }
}
