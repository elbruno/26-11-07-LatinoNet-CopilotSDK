# BYOK: utiliza tu propio proveedor

Cuatro proyectos independientes envían una pregunta breve en español al
proveedor elegido. Compila desde `demos` con
`.\scripts\Verify-Offline.ps1 -Restore`. Para todos ellos se necesita Copilot
CLI; cada ejecución hace una llamada real al proveedor.

| Proveedor | Configuración | Protocolo |
|---|---|---|
| Microsoft Foundry | User Secrets: `Foundry:BaseUrl`, `Foundry:Model` y, opcionalmente, `Foundry:ApiKey`; sin clave usa Azure CLI y Entra. | Responses |
| Ollama | Variables `OLLAMA_BASE_URL` y `OLLAMA_MODEL`; servicio y modelo local existentes. | Chat Completions |
| Foundry Local | Variables `FOUNDRY_LOCAL_BASE_URL` y `FOUNDRY_LOCAL_MODEL`; servicio y modelo local existentes. | Chat Completions |
| OpenAI | User Secrets: `OpenAI:Model` y `OpenAI:ApiKey`. | Responses |

Para Foundry u OpenAI ejecuta `.\Set-DemoSecrets.ps1` desde `demos`, fuera de
cámara. El menú pide la clave con entrada oculta. User Secrets almacena
valores en texto plano fuera del repositorio y es solo para desarrollo.
Foundry requiere la URL HTTPS del recurso de inferencia, no la URL de un
proyecto o del portal. Entra requiere una sesión de Azure CLI ya iniciada.
La clave de OpenAI no es una clave de Foundry ni una suscripción de ChatGPT.
Los modelos sugeridos deben existir en tu cuenta.

Para los proveedores locales, usa la URL real del endpoint compatible `/v1/`
y el identificador exacto del modelo cargado. Consulta el servicio existente:
por ejemplo, `ollama list` y `foundry server status -o json`; verifica el
endpoint y los modelos disponibles de tu instalación. En una terminal ya
preparada, define las variables sin inventar el puerto ni el modelo:

```powershell
$env:OLLAMA_BASE_URL = Read-Host 'URL del endpoint Ollama /v1/'
$env:OLLAMA_MODEL = Read-Host 'Identificador del modelo Ollama'
$env:FOUNDRY_LOCAL_BASE_URL = Read-Host 'URL del endpoint Foundry Local /v1/'
$env:FOUNDRY_LOCAL_MODEL = Read-Host 'Identificador del modelo Foundry Local'
```

Los ejemplos no arrancan servicios ni descargan modelos. Comprueba que el
modelo local sea compatible con las capacidades requeridas por el runtime.
Desde `demos`, ejecuta el proveedor configurado:

```powershell
dotnet run --no-build --project ".\byok\01 - Microsoft Foundry\MicrosoftFoundry.csproj"
dotnet run --no-build --project ".\byok\02 - Ollama\Ollama.csproj"
dotnet run --no-build --project ".\byok\03 - Foundry Local\FoundryLocal.csproj"
dotnet run --no-build --project ".\byok\04 - OpenAI\OpenAi.csproj"
```

`Type = "openai"` describe el protocolo y no implica que todos los ejemplos
usen el servicio de OpenAI. La opción BYOK desactiva el uso de la sesión
almacenada como alternativa, pero no elimina todos los tokens heredados del
proceso. No imprimas el entorno ni compartas credenciales.
