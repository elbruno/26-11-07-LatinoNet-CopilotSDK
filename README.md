# Microsoft Copilot SDK en Latino .NET

Material de la sesión en español de Latino .NET: presentación y ejemplos para
explorar cómo integrar Copilot en aplicaciones y agentes, desde el primer
mensaje hasta herramientas, MCP y proveedores de modelos.

## Material y recorrido

1. [Presentación en PDF](slides/LatinoNET-CopilotSDK-baseline-ES-v08.pdf).
2. [El mismo patrón en cinco lenguajes](demos/examples-lang/README.md): C#,
   Python, Go, TypeScript y Rust.
3. [Conceptos del SDK en C#](demos/csharp-sdk-concepts/README.md): cliente,
   sesiones, streaming, instrucciones, herramientas, permisos, MCP, skills
   y WorkIQ.
4. [BYOK: utiliza tu propio proveedor](demos/byok/README.md): Microsoft
   Foundry, Ollama, Foundry Local y OpenAI.
5. [Ejemplos de aplicaciones .NET](demos/examples-app/README.md): conversación
   con contexto, acciones, una web, comparación de proveedores y resumen de
   un documento. Son material adicional para practicar.

El producto se anunció como **Microsoft Copilot SDK**, anteriormente GitHub
Copilot SDK. La transición de nombre es gradual: los ejemplos conservan los
identificadores técnicos existentes, incluido el paquete `GitHub.Copilot.SDK`,
el namespace `GitHub.Copilot`, los imports y el comando `copilot`.

## Requisitos

- .NET SDK 10 y PowerShell 7 (`pwsh`) para las demos .NET y los scripts.
- GitHub Copilot CLI (`copilot` en el `PATH`), una cuenta autenticada con acceso
  al servicio y un modelo disponible para las llamadas al runtime. Las
  llamadas reales pueden consumir cuota y requieren conexión y autorización.
- Para el recorrido multilenguaje: Python, Go, Node.js con npm y Rust con
  rustup. Los ejemplos fijan las dependencias en sus respectivos manifiestos
  y archivos de bloqueo.
- Para MCP: Node.js y acceso de red a Wikipedia. WorkIQ requiere además el
  plugin instalado, acceso autorizado a Microsoft 365 y una cuenta preparada.
- Para BYOK: acceso autorizado al proveedor elegido y su modelo. Los
  proveedores locales requieren un servicio y un modelo ya disponibles.

La secuencia y los comandos de PowerShell se muestran para Windows. Consulta
los README de cada grupo para conocer sus requisitos específicos.

## Preparación y compilación

Clona el repositorio y entra en `demos`:

```powershell
git clone https://github.com/elbruno/26-11-07-LatinoNet-CopilotSDK.git
Set-Location .\26-11-07-LatinoNet-CopilotSDK\demos
```

Para restaurar dependencias .NET, compilar y ejecutar las comprobaciones
deterministas sin llamar a un modelo:

```powershell
.\scripts\Verify-Offline.ps1 -Restore
```

La restauración puede necesitar red. Las comprobaciones no prueban credenciales,
disponibilidad de proveedores ni inferencia en vivo. Después de compilar,
ejecuta por ejemplo:

```powershell
dotnet run --no-build --project ".\csharp-sdk-concepts\01 - hola mundo\HolaMundo.csproj"
dotnet run --no-build --project ".\examples-app\01-conversacion-con-contexto\Conversation.csproj"
```

Ambos comandos envían solicitudes reales a Copilot. Para seleccionar
opcionalmente un modelo disponible en las demos de conceptos:

```powershell
$env:COPILOT_DEMO_MODEL = Read-Host 'Identificador del modelo disponible'
```

Prepara las dependencias de los cinco lenguajes por separado antes de la
sesión. Este paso puede instalar paquetes y el toolchain GNU de Rust:

```powershell
.\examples-lang\Run-All.ps1 -SetupOnly
.\examples-lang\Run-All.ps1 -SkipSetup
```

El segundo comando realiza cinco llamadas reales. Consulta
[los comandos individuales](demos/examples-lang/README.md) si solo quieres
ejecutar un lenguaje. Para MCP, instala además sus dependencias locales:

```powershell
npm ci --prefix ".\csharp-sdk-concepts\06 - MCP" --ignore-scripts --no-audit --no-fund
```

## Proveedores y secretos

Solo si vas a utilizar Microsoft Foundry u OpenAI, prepara la configuración
fuera de cámara desde `demos`:

```powershell
.\Set-DemoSecrets.ps1
```

El menú solicita el proveedor, modelo y configuración necesaria. Para Foundry
puedes usar una sesión existente de Azure CLI con Microsoft Entra o una clave
del recurso; OpenAI utiliza su propia clave de API. El script pide las claves
con entrada oculta y no las acepta como argumentos. No guarda claves en este
repositorio: .NET User Secrets las almacena **en texto plano** fuera del
repositorio y es solo para desarrollo, no un almacén seguro de producción.
No compartas ni proyectes valores reales. Foundry comparte su configuración
entre BYOK 01 y la comparación de proveedores.

Para Ollama y Foundry Local configura las variables del proceso indicadas en
la [guía BYOK](demos/byok/README.md), con la URL y el identificador reales
del servicio existente. Estos ejemplos no descargan modelos ni inician
servicios. Verifica los permisos, disponibilidad y costos del proveedor antes
de ejecutar cualquier llamada.

## Permisos y privacidad

Las herramientas tienen permisos limitados por cada ejemplo; una lista
permitida no equivale a aislamiento del sistema operativo. MCP inicia un
proceso local y consulta Wikipedia por red. El ejemplo WorkIQ accede a datos
privados de Microsoft 365 y necesita autorización independiente: revisa su
configuración fuera de cámara y evita proyectar datos personales. No se debe
interpretar la compilación como una validación de acceso en vivo.

## Recursos

- [Repositorio oficial del SDK](https://github.com/github/copilot-sdk).
- [Anuncio del cambio de nombre](https://github.blog/changelog/2026-10-28-github-copilot-sdk-is-becoming-microsoft-copilot-sdk/).
- [GitHub Copilot app](https://github.com/github/app).
- [New Year, New Bot](https://aka.ms/newyearnewbot).
- [Generative AI for Beginners .NET](http://aka.ms/genainet).
- [Foundry Local y agentic coding con Qwen](https://devblogs.microsoft.com/foundry/foundry-local-agentic-coding-qwen3-8-27b/).
- [De dotnet run a Foundry Hosted Agent en C#](https://devblogs.microsoft.com/dotnet/from-dotnet-run-to-foundry-hosted-agent-in-3-lines-of-csharp/).

Consulta los [avisos de terceros](demos/THIRD-PARTY-NOTICES.txt) para las
atribuciones correspondientes.
