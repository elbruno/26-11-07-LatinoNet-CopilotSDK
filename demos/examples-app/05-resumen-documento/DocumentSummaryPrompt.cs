namespace AppliedExamples;

public static class DocumentSummaryPrompt
{
    public static string Build(string document) =>
        $"""
        Resume el siguiente documento sintetico en espanol.
        Luego enumera tareas concretas solo cuando esten respaldadas por el texto.
        No inventes responsables, fechas ni compromisos; indica si un dato no aparece.

        <documento>
        {document}
        </documento>
        """;
}
