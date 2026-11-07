# Ejemplos adicionales de aplicaciones .NET

Compila desde `demos` con `.\scripts\Verify-Offline.ps1 -Restore`. Después,
ejecuta un proyecto a la vez con Copilot CLI autenticado y acceso al modelo.
Cada ejemplo realiza solicitudes reales.

| Ejemplo | Descripción | Comando desde `demos` |
|---|---|---|
| 01 | Tres turnos en una sesión con contexto | `dotnet run --no-build --project ".\examples-app\01-conversacion-con-contexto\Conversation.csproj"` |
| 02 | Consulta acotada de un ticket sintético | `dotnet run --no-build --project ".\examples-app\02-de-texto-a-accion\TextToAction.csproj"` |
| 03 | Web local con respuestas por SSE | `dotnet run --no-build --project ".\examples-app\03-app-web\WebConversation.csproj"` |
| 04 | Una misma pregunta a Copilot y Foundry | `dotnet run --no-build --project ".\examples-app\04-comparar-proveedores\CompareProviders.csproj"` |
| 05 | Resumen de un documento sintético incluido | `dotnet run --no-build --project ".\examples-app\05-resumen-documento\DocumentSummary.csproj"` |

Para la web, abre `http://127.0.0.1:5057` después de iniciar el proyecto y
detén el proceso con Ctrl+C. La comparación no es una evaluación de calidad;
requiere tanto Copilot como Foundry configurado mediante
`.\Set-DemoSecrets.ps1`. La configuración de Foundry se comparte con BYOK 01.
