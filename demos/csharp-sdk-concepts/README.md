# Conceptos del SDK en C#

Ocho proyectos independientes `net10.0` muestran una capacidad por vez.
Conservan el paquete `GitHub.Copilot.SDK` y el namespace `GitHub.Copilot`.
Desde `demos`, compila con `.\scripts\Verify-Offline.ps1 -Restore` y ejecuta
un proyecto a la vez. Cada ejecución inicia Copilot y realiza una solicitud real.

| Proyecto | Concepto | Comando desde `demos` |
|---|---|---|
| 01 | Cliente, sesión y respuesta | `dotnet run --no-build --project ".\csharp-sdk-concepts\01 - hola mundo\HolaMundo.csproj"` |
| 02 | Streaming | `dotnet run --no-build --project ".\csharp-sdk-concepts\02 - streaming\Streaming.csproj"` |
| 03 | Instrucciones de sistema | `dotnet run --no-build --project ".\csharp-sdk-concepts\03 - system prompt\SystemPrompt.csproj"` |
| 04 | Herramienta local acotada | `dotnet run --no-build --project ".\csharp-sdk-concepts\04 - tool local\ToolLocal.csproj"` |
| 05 | Permisos explícitos | `dotnet run --no-build --project ".\csharp-sdk-concepts\05 - permisos\Permisos.csproj"` |
| 06 | MCP y Wikipedia | `dotnet run --no-build --project ".\csharp-sdk-concepts\06 - MCP\Mcp.csproj"` |
| 07 | Skill local | `dotnet run --no-build --project ".\csharp-sdk-concepts\07 - skills\Skills.csproj"` |
| 08 | WorkIQ | `dotnet run --no-build --project ".\csharp-sdk-concepts\08 - WorkIQ\WorkIq.csproj"` |

Opcionalmente, define `$env:COPILOT_DEMO_MODEL` con un modelo disponible;
sin esa variable se usa el valor predeterminado del runtime.

Para MCP, instala primero su servidor local y ejecuta desde `demos`:

```powershell
npm ci --prefix ".\csharp-sdk-concepts\06 - MCP" --ignore-scripts --no-audit --no-fund
```

MCP consulta Wikipedia por red. Para WorkIQ, establece `WORKIQ_PLUGIN_DIR`
con la ruta del plugin ya instalado que contiene `.mcp.json` y skills.
WorkIQ necesita autorización y acceso a Microsoft 365, y puede consultar datos
privados. Prepara la cuenta fuera de cámara y no proyectes datos privados.
La lista de herramientas permitidas no constituye aislamiento del sistema.
