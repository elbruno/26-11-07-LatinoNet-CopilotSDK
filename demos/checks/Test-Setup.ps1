# No real credentials: intercept dotnet, then use an isolated temporary APPDATA.
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$root = Split-Path $PSScriptRoot
$global:setupCheckPassed = 0
$global:setupCheckValues = $null
$global:setupCheckProject = $null
$global:setupCheckAnswers = @()
$global:setupCheckAnswerIndex = 0
function Assert([bool]$Condition, [string]$Label) {
    if (-not $Condition) { throw "FAIL: $Label" }
    $global:setupCheckPassed++
}
function Read-Host {
    param([string]$Prompt, [switch]$AsSecureString)
    if ($AsSecureString) {
        return ConvertTo-SecureString 'offline-placeholder-not-a-real-key' -AsPlainText -Force
    }
    if ($global:setupCheckAnswerIndex -ge $global:setupCheckAnswers.Count) {
        throw "Unexpected interactive prompt: $Prompt"
    }
    return $global:setupCheckAnswers[$global:setupCheckAnswerIndex++]
}
function dotnet {
    $arguments = $args
    if (($arguments[0..2] -join ' ') -ne 'user-secrets set --project') {
        throw 'Unexpected dotnet command intercepted.'
    }
    $global:setupCheckProject = $arguments[3]
    $global:setupCheckValues = ($input | Out-String) | ConvertFrom-Json -AsHashtable
    Assert ($arguments.Count -eq 4) 'No key/value in process arguments'
    $global:LASTEXITCODE = 0
}
# No arguments: bad menu/endpoint/auth choices retry, Enter selects documented defaults.
$global:setupCheckAnswers = @('', '', 'not-an-endpoint', 'https://example.invalid/openai/v1/', '9', '')
$global:setupCheckAnswerIndex = 0
& (Join-Path $root 'Set-DemoSecrets.ps1')
Assert ($global:setupCheckAnswerIndex -eq $global:setupCheckAnswers.Count) 'Interactive prompts are explicit and complete'
Assert ($global:setupCheckValues['Foundry:BaseUrl'] -eq 'https://example.invalid/openai/v1/') 'Foundry endpoint'
Assert ($global:setupCheckValues['Foundry:Model'] -eq 'gpt-6-luna') 'Foundry default model'
Assert ($global:setupCheckValues['Foundry:ApiKey'] -eq '') 'Entra clears optional key explicitly'
Assert ($global:setupCheckProject.EndsWith('MicrosoftFoundry.csproj')) 'Foundry project mapping'

# Explicit mode remains scriptable; only the API key is requested securely.
$global:setupCheckAnswers = @()
$global:setupCheckAnswerIndex = 0
& (Join-Path $root 'Set-DemoSecrets.ps1') -Provider Foundry -Model 'test-deployment' -ResourceUrl 'https://example.invalid' -UseApiKey
Assert ($global:setupCheckAnswerIndex -eq 0) 'Explicit parameters skip interactive menus'
Assert ($global:setupCheckValues['Foundry:ApiKey'] -eq 'offline-placeholder-not-a-real-key') 'Foundry optional API key'
Assert ($global:setupCheckValues['Foundry:Model'] -eq 'test-deployment') 'Explicit deployment'

$global:setupCheckAnswers = @('2', '')
$global:setupCheckAnswerIndex = 0
& (Join-Path $root 'Set-DemoSecrets.ps1')
Assert ($global:setupCheckAnswerIndex -eq 2) 'OpenAI interactive choices'
Assert ($global:setupCheckValues['OpenAI:Model'] -eq 'gpt-4.1-mini') 'OpenAI suggested model default'
Assert ($global:setupCheckValues['OpenAI:ApiKey'] -eq 'offline-placeholder-not-a-real-key') 'OpenAI masked input'
Assert ($global:setupCheckProject.EndsWith('OpenAi.csproj')) 'OpenAI project mapping'
foreach ($url in @('http://example.invalid', 'https://example.invalid/api/projects/p', 'https://example.invalid/?key=x')) {
    $rejected = $false
    try { & (Join-Path $root 'Set-DemoSecrets.ps1') -Provider Foundry -ResourceUrl $url }
    catch { $rejected = $true }
    Assert $rejected 'Reject wrong Foundry endpoint'
}
$global:setupCheckValues = $null
& (Join-Path $root 'Set-DemoSecrets.ps1') -Provider OpenAI -Model test -WhatIf
Assert ($null -eq $global:setupCheckValues) 'WhatIf never writes or prompts for secret'

Remove-Item Function:\dotnet
$oldAppData = $env:APPDATA
$temporary = Join-Path ([IO.Path]::GetTempPath()) ('latinonet-secret-check-' + [Guid]::NewGuid())
$testId = 'offline-' + [Guid]::NewGuid()
New-Item -ItemType Directory -Path $temporary | Out-Null
try {
    $env:APPDATA = $temporary
    # Real CLI transport with dummy JSON only. Unique ID is not a demo/user ID.
    '{"Dummy:Key":"offline-placeholder-not-a-real-key"}' |
        & dotnet user-secrets set --id $testId
    if ($LASTEXITCODE -ne 0) { throw 'Isolated CLI stdin check failed.' }
    $path = Join-Path $temporary "Microsoft\UserSecrets\$testId\secrets.json"
    Assert (Test-Path -LiteralPath $path) 'CLI honors isolated APPDATA'
    $values = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json -AsHashtable
    Assert ($values['Dummy:Key'] -eq 'offline-placeholder-not-a-real-key') 'Real CLI stdin JSON roundtrip'

    # Exercise the actual setup script with synthetic values and isolated storage.
    # The Foundry ID is shared by the existing BYOK project and the new comparison sample.
    $foundryProject = Join-Path $root 'byok\01 - Microsoft Foundry\MicrosoftFoundry.csproj'
    $compareProject = Join-Path $root 'examples-app\04-comparar-proveedores\CompareProviders.csproj'
    $openAiProject = Join-Path $root 'byok\04 - OpenAI\OpenAi.csproj'
    & (Join-Path $root 'Set-DemoSecrets.ps1') -Provider Foundry `
        -Model 'offline-synthetic-model' -ResourceUrl 'https://example.invalid' -UseApiKey
    $foundryOutput = (& dotnet user-secrets list --project $foundryProject | Out-String)
    if ($LASTEXITCODE -ne 0) { throw 'Foundry secrets verification failed.' }
    $compareOutput = (& dotnet user-secrets list --project $compareProject | Out-String)
    if ($LASTEXITCODE -ne 0) { throw 'Provider comparison secrets verification failed.' }
    Assert ($foundryOutput -match 'Foundry:ApiKey = offline-placeholder-not-a-real-key') 'Foundry consumer receives synthetic key'
    Assert ($compareOutput -eq $foundryOutput) 'Applied comparison reads the same Foundry secrets'
    Assert ($foundryOutput -match 'Foundry:Model = offline-synthetic-model') 'Foundry consumer receives synthetic model'

    & (Join-Path $root 'Set-DemoSecrets.ps1') -Provider OpenAI -Model 'offline-synthetic-model'
    $openAiOutput = (& dotnet user-secrets list --project $openAiProject | Out-String)
    if ($LASTEXITCODE -ne 0) { throw 'OpenAI secrets verification failed.' }
    Assert ($openAiOutput -match 'OpenAI:ApiKey = offline-placeholder-not-a-real-key') 'OpenAI consumer receives synthetic key'
    Assert ($openAiOutput -match 'OpenAI:Model = offline-synthetic-model') 'OpenAI consumer receives synthetic model'
}
finally {
    $env:APPDATA = $oldAppData
    # Exact newly-created test directory, never a user secrets directory.
    Remove-Item -LiteralPath $temporary -Recurse -Force
}
foreach ($group in @('byok', 'csharp-sdk-concepts')) {
    $programs = @(Get-ChildItem -LiteralPath (Join-Path $root $group) -Directory |
        ForEach-Object { Get-ChildItem -LiteralPath $_.FullName -Filter 'Program.cs' -File })
    Assert ($programs.Count -eq $(if ($group -eq 'byok') { 4 } else { 8 })) "$group count"
    foreach ($program in $programs) {
        $text = Get-Content -LiteralPath $program.FullName -Raw
        Assert ($text -cnotmatch '\bargs\b|Console\.ReadLine|--stage|--run') 'No demo console modes'
        Assert ($text -match 'await using var client' -and $text -match 'await using var session') 'Async disposal'
    }
}
foreach ($project in Get-ChildItem -LiteralPath (Join-Path $root 'examples-app') -Directory) {
    $program = Join-Path $project.FullName 'Program.cs'
    if (-not (Test-Path -LiteralPath $program)) { continue }
    $text = Get-Content -LiteralPath $program -Raw
    Assert ($text -cnotmatch '\bargs\b|--stage|--run') 'Applied demos have no application modes'
}
foreach ($projectName in @('CompareProviders.csproj')) {
    $project = Get-ChildItem -LiteralPath (Join-Path $root 'examples-app') -Filter $projectName -File -Recurse
    $projectXml = [xml](Get-Content -LiteralPath $project.FullName -Raw)
    Assert ($projectXml.Project.PropertyGroup.UserSecretsId -eq 'latinonet-copilot-sdk-foundry-v02') 'Provider comparison reuses Foundry secrets'
}
Assert (@(Get-ChildItem -LiteralPath $root -Filter '*.ps1' -File).Count -eq 1) 'One root PowerShell'
Write-Host "PASS: $global:setupCheckPassed setup/structure checks; isolated synthetic credentials only."
