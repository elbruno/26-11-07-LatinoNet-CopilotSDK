namespace AppliedExamples;

public static class WebTurnExecution
{
    public static async Task RunAsync(
        Func<CancellationToken, Task> sendAsync,
        Func<CancellationToken, Task> abortAsync,
        CancellationToken cancellationToken)
    {
        try
        {
            await sendAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await abortAsync(CancellationToken.None);
            throw;
        }
    }
}
