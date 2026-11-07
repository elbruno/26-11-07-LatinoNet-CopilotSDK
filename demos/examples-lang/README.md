# Un ejemplo en cinco lenguajes

Los ejemplos muestran el mismo patrón de cliente, sesión y mensaje en C#,
Python, Go, TypeScript y Rust. Están adaptados del
[workshop oficial](https://github.com/github/copilot-sdk-workshop/tree/716bdaaf629817606873b4e22d551e900985c92c)
y usan la versión 1.0.11 del SDK.

Desde `demos`, prepara dependencias antes de ejecutar:

```powershell
.\examples-lang\Run-All.ps1 -SetupOnly
.\examples-lang\Run-All.ps1 -SkipSetup
```

La primera línea instala dependencias y puede instalar el toolchain GNU de
Rust; revisa el script antes de ejecutarla. La segunda hace cinco llamadas
reales a modelos. Para ejecutar por separado, desde `demos`:

```powershell
dotnet run --project ".\examples-lang\csharp\CopilotSdkHello.csproj"

Push-Location .\examples-lang\python
.\.venv\Scripts\python.exe .\main.py
Pop-Location

go -C .\examples-lang\go run .
npm --prefix .\examples-lang\typescript start
cargo +stable-x86_64-pc-windows-gnu run --locked --manifest-path .\examples-lang\rust\Cargo.toml
```

Cada lenguaje necesita su toolchain y sus dependencias preparados.
