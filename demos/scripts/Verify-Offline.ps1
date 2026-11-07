param(
    [switch]$Restore
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$demoRoot = Split-Path $PSScriptRoot

function Invoke-Dotnet {
    param([string[]]$Arguments)
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

$projects = @(
    'examples-lang\csharp\CopilotSdkHello.csproj'
    'checks\OfflineChecks.csproj'
)
$projects += foreach ($group in @('csharp-sdk-concepts', 'byok', 'examples-app')) {
    Get-ChildItem -LiteralPath (Join-Path $demoRoot $group) -Directory |
        ForEach-Object { Get-ChildItem -LiteralPath $_.FullName -Filter '*.csproj' -File } |
        ForEach-Object { [IO.Path]::GetRelativePath($demoRoot, $_.FullName) }
}

foreach ($project in $projects) {
    $path = Join-Path $demoRoot $project
    Write-Host "=== $project ==="
    if ($Restore) {
        Invoke-Dotnet -Arguments @('restore', $path, '--locked-mode', '--nologo', '-v:q')
    }
    Invoke-Dotnet -Arguments @('build', $path, '--no-restore', '--nologo', '-v:q')
}

Invoke-Dotnet -Arguments @(
    'run', '--no-build', '--no-restore',
    '--project', (Join-Path $demoRoot 'checks\OfflineChecks.csproj')
)

Write-Host "PASS: $($projects.Count) .NET builds and deterministic offline contracts."
& pwsh -NoProfile -File (Join-Path $demoRoot 'checks\Test-Setup.ps1')
if ($LASTEXITCODE -ne 0) { throw 'Offline setup checks failed.' }
Write-Host 'No Copilot runtime, account preflight, real user secrets, model or external tool was invoked.'
