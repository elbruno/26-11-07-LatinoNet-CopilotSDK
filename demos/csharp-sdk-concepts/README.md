# Conceptos del SDK en C#

Ocho proyectos independientes con .NET 10 y `GitHub.Copilot.SDK` 1.0.11.
Después de preparar el runtime y compilar con `.\scripts\Verify-Offline.ps1
-Restore`, ejecuta desde `demos`:

| Ejemplo | Concepto | Comando |
|---|---|---|
| 01 | Cliente, sesión y respuesta | `dotnet run --no-build --project ".\csharp-sdk-concepts\01 - hola mundo\HolaMundo.csproj"` |
| 02 | Streaming | `dotnet run --no-build --project ".\csharp-sdk-concepts\02 - streaming\Streaming.csproj"` |
| 03 | Instrucciones de sistema | `dotnet run --no-build --project ".\csharp-sdk-concepts\03 - system prompt\SystemPrompt.csproj"` |
| 04 | Herramienta local | `dotnet run --no-build --project ".\csharp-sdk-concepts\04 - tool local\ToolLocal.csproj"` |
| 05 | Permisos para herramientas | `dotnet run --no-build --project ".\csharp-sdk-concepts\05 - permisos\Permisos.csproj"` |
| 06 | MCP con Wikipedia | `dotnet run --no-build --project ".\csharp-sdk-concepts\06 - MCP\Mcp.csproj"` |
| 07 | Skills locales | `dotnet run --no-build --project ".\csharp-sdk-concepts\07 - skills\Skills.csproj"` |
| 08 | WorkIQ | `dotnet run --no-build --project ".\csharp-sdk-concepts\08 - WorkIQ\WorkIq.csproj"` |

Para MCP prepara las dependencias con `npm ci --prefix
".\csharp-sdk-concepts\06 - MCP" --ignore-scripts --no-audit --no-fund`.
WorkIQ necesita un plugin ya instalado, `WORKIQ_PLUGIN_DIR`, autorización y
acceso a Microsoft 365. No compartas datos privados. Las herramientas
permitidas no constituyen aislamiento del sistema operativo.
