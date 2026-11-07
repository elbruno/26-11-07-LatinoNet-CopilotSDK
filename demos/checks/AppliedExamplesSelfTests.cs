using System.Text.Json;
using AppliedExamples;

public static class AppliedExamplesSelfTests
{
    public static void Run()
    {
        var checks = 0;
        var received = new List<string>();
        var answers = ConversationSteps.RunAsync(prompt =>
        {
            received.Add(prompt);
            return Task.FromResult($"turn-{received.Count}");
        }).GetAwaiter().GetResult();

        Check(received.Count == 3, "three context turns", ref checks);
        Check(answers.SequenceEqual(["turn-1", "turn-2", "turn-3"]), "sequential same-session callback", ref checks);
        Check(received[1].Contains("15 minutos", StringComparison.Ordinal), "second turn changes demo duration", ref checks);
        Check(received[2].Contains("sesion remota", StringComparison.Ordinal), "third turn preserves remote format", ref checks);

        Check(SupportTicketCatalog.Lookup("LAT-1042").Contains("filtro de fechas", StringComparison.Ordinal), "synthetic ticket lookup", ref checks);
        Check(SupportTicketCatalog.Lookup("unknown").Contains("No existe", StringComparison.Ordinal), "unknown ticket is bounded", ref checks);
        Check(SupportTicketCatalog.AllowsTool(SupportTicketCatalog.ToolName), "known tool permission", ref checks);
        Check(!SupportTicketCatalog.AllowsTool("other_tool"), "unknown tool permission rejected", ref checks);

        var frame = SseProtocol.Format("delta", "hola\nmundo");
        Check(frame.StartsWith("event: delta\ndata: ", StringComparison.Ordinal), "SSE event frame", ref checks);
        var json = frame.Split('\n')[1]["data: ".Length..];
        Check(JsonDocument.Parse(json).RootElement.GetString() == "hola\nmundo", "SSE JSON escapes newlines", ref checks);

        using var cancellation = new CancellationTokenSource();
        var abortCalled = false;
        var cancelled = false;
        try
        {
            WebTurnExecution.RunAsync(
                async token =>
                {
                    cancellation.Cancel();
                    await Task.Delay(Timeout.Infinite, token);
                },
                _ =>
                {
                    abortCalled = true;
                    return Task.CompletedTask;
                },
                cancellation.Token).GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
            cancelled = true;
        }
        Check(cancelled && abortCalled, "web request cancellation aborts SDK turn", ref checks);

        var document = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "sample-workshop-brief.md"));
        var prompt = DocumentSummaryPrompt.Build(document);
        Check(prompt.Contains("Taller ficticio", StringComparison.Ordinal), "sample document included in prompt", ref checks);
        Check(prompt.Contains("No inventes responsables", StringComparison.Ordinal), "document summary constrains unsupported tasks", ref checks);

        Console.WriteLine($"[applied-self-test] PASS {checks} checks; no runtime or model calls.");
    }

    private static void Check(bool condition, string name, ref int checks)
    {
        if (!condition) throw new InvalidOperationException($"FAIL: {name}");
        checks++;
    }
}
