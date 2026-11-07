# Ejemplos de aplicaciones .NET

Cinco proyectos independientes usan `GitHub.Copilot.SDK` 1.0.11. Desde `demos`,
prepara primero el entorno y compila con `.\scripts\Verify-Offline.ps1 -Restore`.
Cada ejecución realiza una llamada real al modelo.

| Ejemplo | Descripción | Comando |
|---|---|---|
| 01 | Tres turnos en una sesión, con contexto | `dotnet run --no-build --project ".\examples-app\01-conversacion-con-contexto\Conversation.csproj"` |
| 02 | Ticket ficticio, herramienta local y permiso limitado | `dotnet run --no-build --project ".\examples-app\02-de-texto-a-accion\TextToAction.csproj"` |
| 03 | Aplicación web con streaming | `dotnet run --no-build --project ".\examples-app\03-app-web\WebConversation.csproj"` |
| 04 | Comparación de Copilot y Foundry | `dotnet run --no-build --project ".\examples-app\04-comparar-proveedores\CompareProviders.csproj"` |
| 05 | Resumen de un documento de ejemplo | `dotnet run --no-build --project ".\examples-app\05-resumen-documento\DocumentSummary.csproj"` |

Para el ejemplo 03 abre http://127.0.0.1:5057 y usa Ctrl+C para terminar.
Para el 04 configura Foundry con `.\Set-DemoSecrets.ps1`; comparte la
configuración con el ejemplo BYOK 01. Necesita además el runtime de Copilot y
una sesión Entra autorizada o la clave del recurso Foundry. La comparación es
ilustrativa, no una evaluación formal de modelos.
