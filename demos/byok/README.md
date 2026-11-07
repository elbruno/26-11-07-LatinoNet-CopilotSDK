# BYOK: usar tus propios proveedores

Cuatro consolas independientes usan .NET 10 y `GitHub.Copilot.SDK` 1.0.11.
Después de restaurar y compilar con `.\scripts\Verify-Offline.ps1 -Restore`,
ejecuta los comandos desde `demos`. Cada ejecución realiza inferencia real.

| Proveedor | Configuración | Comando |
|---|---|---|
| Microsoft Foundry | User Secrets: `Foundry:BaseUrl`, `Foundry:Model` y clave opcional, o Entra | `dotnet run --no-build --project ".\byok\01 - Microsoft Foundry\MicrosoftFoundry.csproj"` |
| Ollama | `OLLAMA_BASE_URL` y `OLLAMA_MODEL` | `dotnet run --no-build --project ".\byok\02 - Ollama\Ollama.csproj"` |
| Foundry Local | `FOUNDRY_LOCAL_BASE_URL` y `FOUNDRY_LOCAL_MODEL` | `dotnet run --no-build --project ".\byok\03 - Foundry Local\FoundryLocal.csproj"` |
| OpenAI | User Secrets: `OpenAI:Model` y `OpenAI:ApiKey` | `dotnet run --no-build --project ".\byok\04 - OpenAI\OpenAi.csproj"` |

Para Foundry u OpenAI, usa `.\Set-DemoSecrets.ps1`: el menú muestra los
proveedores y métodos de autenticación. La clave se solicita de forma oculta.
El modelo y endpoint deben existir en tu cuenta. User Secrets almacena valores
en texto plano fuera del repositorio y no reemplaza un almacén de producción.

Para los proveedores locales, usa el endpoint y el ID de modelo reales del
servicio que ya tienes preparado. No inventes el puerto de Foundry Local:
consulta `foundry server status -o json` y los modelos disponibles del
servidor. Por ejemplo, después de identificar los valores correspondientes:

```powershell
$env:OLLAMA_BASE_URL = 'http://127.0.0.1:11434/v1/'
$env:OLLAMA_MODEL = '<modelo-instalado>'
$env:FOUNDRY_LOCAL_BASE_URL = '<endpoint-real-con-v1>'
$env:FOUNDRY_LOCAL_MODEL = '<id-real-del-modelo>'
```

Sustituye los marcadores y el puerto de Ollama si tu servicio usa otro.
El SDK utiliza `Type = "openai"` para describir un protocolo compatible,
no necesariamente el proveedor OpenAI.

[Foundry Local CLI](https://learn.microsoft.com/azure/foundry-local/reference/reference-cli) |
[Compatibilidad de Ollama](https://docs.ollama.com/api/openai-compatibility) |
[Autenticación de OpenAI](https://platform.openai.com/docs/api-reference/authentication)
