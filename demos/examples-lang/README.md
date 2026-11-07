# Un primer ejemplo en cinco lenguajes

Los ejemplos muestran el mismo patrón de cliente, sesión y mensaje en C#,
Python, Go, TypeScript y Rust. Están adaptados del
[taller oficial](https://github.com/github/copilot-sdk-workshop/tree/716bdaaf629817606873b4e22d551e900985c92c)
y usan la versión 1.0.11 del SDK. El nuevo nombre del producto no cambia los
identificadores técnicos existentes.

Desde `demos`, prepara las dependencias antes de la sesión y después ejecuta:

```powershell
.\examples-lang\Run-All.ps1 -SetupOnly
.\examples-lang\Run-All.ps1 -SkipSetup
```

La preparación puede instalar dependencias y el toolchain GNU de Rust. La
ejecución envía cinco solicitudes reales; requiere Copilot CLI autenticado,
acceso al modelo, red y cuota. `-SkipSetup` no instala dependencias.

Para ejecutar un ejemplo por separado, estos comandos parten de `demos`:

```powershell
# C#
dotnet restore .\examples-lang\csharp\CopilotSdkHello.csproj --locked-mode
dotnet run --no-restore --project .\examples-lang\csharp\CopilotSdkHello.csproj

# Python
python -m venv .\examples-lang\python\.venv
.\examples-lang\python\.venv\Scripts\python.exe -m pip install -e .\examples-lang\python
.\examples-lang\python\.venv\Scripts\python.exe .\examples-lang\python\main.py

# Go
go -C .\examples-lang\go mod download
go -C .\examples-lang\go run .

# TypeScript
npm --prefix .\examples-lang\typescript ci
npm --prefix .\examples-lang\typescript start

# Rust en Windows
rustup toolchain install stable-x86_64-pc-windows-gnu --profile minimal
cargo +stable-x86_64-pc-windows-gnu run --locked --manifest-path .\examples-lang\rust\Cargo.toml
```
