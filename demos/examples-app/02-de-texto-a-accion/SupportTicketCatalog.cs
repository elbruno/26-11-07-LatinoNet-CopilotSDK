using System.Collections.Generic;

namespace AppliedExamples;

public static class SupportTicketCatalog
{
    public const string ToolName = "lookup_support_ticket";

    private static readonly IReadOnlyDictionary<string, string> Tickets =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["LAT-1042"] = "La persona no puede abrir el reporte semanal. El error comenzo despues de cambiar el filtro de fechas. No se reporta perdida de datos.",
            ["LAT-1043"] = "La descarga CSV tarda mas de lo habitual al filtrar varios meses. El reporte en pantalla carga correctamente."
        };

    public static string Lookup(string ticketId) =>
        Tickets.TryGetValue(ticketId, out var summary)
            ? $"{ticketId}: {summary}"
            : "No existe ese ticket sintetico en el catalogo local.";

    public static bool AllowsTool(string toolName) =>
        string.Equals(toolName, ToolName, StringComparison.Ordinal);
}
