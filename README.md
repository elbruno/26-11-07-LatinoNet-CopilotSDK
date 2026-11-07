# Microsoft Copilot SDK en .NET y otros lenguajes

Materiales de la sesión de Latino .NET. El repositorio reúne las diapositivas y
ejemplos para explorar sesiones, herramientas, streaming y proveedores de modelos.

## Diapositivas y ejemplos

- [Diapositivas en PDF](slides/LatinoNET-CopilotSDK-baseline-ES-v08.pdf)
- [Ejemplos por lenguaje](demos/examples-lang/README.md): C#, Python, Go,
  TypeScript y Rust.
- [Conceptos del SDK en C#](demos/csharp-sdk-concepts/README.md): ocho consolas
  independientes, de la primera sesión a MCP y skills.
- [Ejemplos de aplicaciones](demos/examples-app/README.md): conversación con
  contexto, herramienta local, aplicación web, comparación de proveedores y
  resumen de documentos.
- [Ejemplos BYOK](demos/byok/README.md): Microsoft Foundry, Ollama, Foundry
  Local y OpenAI.
- [Avisos de terceros](demos/THIRD-PARTY-NOTICES.txt).

## Requisitos

- .NET SDK 10 y PowerShell 7 (`pwsh`) para las consolas y scripts.
- CLI de Copilot (`copilot`) instalada y autenticada para ejecutar los ejemplos
  que usan el servicio. Las llamadas al modelo pueden consumir cuota.
- Según el lenguaje: Python, Go, Node.js y npm, o Rust y Cargo. El script
  multilenguaje usa el toolchain GNU de Rust en Windows.
- Para BYOK, una cuenta, modelo o servicio local ya configurado según el
  proveedor elegido. Foundry con Entra requiere una sesión de Azure CLI
  autorizada, salvo que se use una clave del recurso.

El producto se anunció como **Microsoft Copilot SDK**, anteriormente GitHub
Copilot SDK. La transición de nombres es gradual: los ejemplos conservan
identificadores técnicos como `GitHub.Copilot.SDK` y `GitHub.Copilot`.

## Primeros pasos

```powershell
git clone https://github.com/elbruno/26-11-07-LatinoNet-CopilotSDK.git
Set-Location .\26-11-07-LatinoNet-CopilotSDK\demos

# Restaura, compila y ejecuta comprobaciones locales sin llamar al modelo.
.\scripts\Verify-Offline.ps1 -Restore

# Ejecuta una primera llamada real al modelo.
dotnet run --no-build --project ".\csharp-sdk-concepts\01 - hola mundo\HolaMundo.csproj"
```

La comprobación offline no demuestra que la autenticación, los modelos o los
proveedores estén disponibles. Comprueba esos requisitos antes de una presentación.

### Cinco lenguajes

Después de preparar las dependencias, desde `demos`:

```powershell
.\examples-lang\Run-All.ps1 -SkipSetup
```

Este comando efectúa cinco llamadas reales. La guía de
[lenguajes](demos/examples-lang/README.md) explica la preparación y ejecución
individual. `-SetupOnly` puede instalar dependencias y el toolchain GNU de Rust.

### Aplicaciones en C#

```powershell
# Conversación con contexto en una misma sesión.
dotnet run --no-build --project ".\examples-app\01-conversacion-con-contexto\Conversation.csproj"

# Herramienta local con datos ficticios.
dotnet run --no-build --project ".\examples-app\02-de-texto-a-accion\TextToAction.csproj"

# Aplicación web con streaming. Abre http://127.0.0.1:5057 y detén con Ctrl+C.
dotnet run --no-build --project ".\examples-app\03-app-web\WebConversation.csproj"

# Comparación entre Copilot y Microsoft Foundry ya configurado.
dotnet run --no-build --project ".\examples-app\04-comparar-proveedores\CompareProviders.csproj"

# Resumen de un documento de ejemplo incluido.
dotnet run --no-build --project ".\examples-app\05-resumen-documento\DocumentSummary.csproj"
```

La comparación es ilustrativa, no una prueba estadística de calidad. La
aplicación web envía las peticiones desde el backend, no desde el navegador con
una clave de proveedor.

## Configuración de secretos y BYOK

Desde `demos`, ejecuta antes de compartir pantalla:

```powershell
.\Set-DemoSecrets.ps1
```

El menú permite configurar Microsoft Foundry u OpenAI. Para Foundry elige
autenticación Entra o una clave del recurso. Los modelos sugeridos son ejemplos:
usa un identificador disponible en tu cuenta. La clave se solicita de forma
oculta y no tiene valor predeterminado. La configuración de Foundry es compartida
por BYOK 01 y la comparación de proveedores.

Los .NET User Secrets se almacenan en texto plano fuera del repositorio y son
para desarrollo, no un almacén seguro de producción. No compartas ni confirmes
claves en Git. Consulta [BYOK](demos/byok/README.md) para Ollama, Foundry Local
y los comandos específicos de cada proveedor.

## MCP y WorkIQ

El ejemplo MCP usa un servidor de Wikipedia. Para preparar sus dependencias,
desde `demos`:

```powershell
npm ci --prefix ".\csharp-sdk-concepts\06 - MCP" --ignore-scripts --no-audit --no-fund
```

MCP necesita conectividad y permisos adecuados. WorkIQ requiere el plugin ya
instalado, configuración y autorización independiente para Microsoft 365.
Puede acceder a datos privados: usa únicamente información y cuentas para las
que tengas permiso, y no proyectes resultados privados.

## Recursos

- [Repositorio oficial del SDK](https://github.com/github/copilot-sdk).
- [Repositorio de la aplicación GitHub Copilot](https://github.com/github/app).
- [Anuncio del cambio de nombre](https://github.blog/changelog/2026-10-28-github-copilot-sdk-is-becoming-microsoft-copilot-sdk/).
- [New Year, New Bot](https://aka.ms/newyearnewbot).
- [Generative AI for .NET](https://aka.ms/genainet).
- [Foundry Local y agentic coding](https://devblogs.microsoft.com/foundry/foundry-local-agentic-coding-qwen3-8-27b/).
- [De dotnet run a Foundry Hosted Agent](https://devblogs.microsoft.com/dotnet/from-dotnet-run-to-foundry-hosted-agent-in-3-lines-of-csharp/).
- [Workshop del Copilot SDK](https://github.github.com/copilot-sdk-workshop/), recurso opcional.
