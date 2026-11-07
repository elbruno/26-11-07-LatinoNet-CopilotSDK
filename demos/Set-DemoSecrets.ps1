<#
.SYNOPSIS
Configures BYOK demo User Secrets using hidden API-key input.

.DESCRIPTION
Foundry settings are shared by BYOK 01 and the applied provider-comparison
example 04. OpenAI settings are used by BYOK 04. The script never prints or
accepts an API key as a parameter.

.EXAMPLE
.\Set-DemoSecrets.ps1

.EXAMPLE
.\Set-DemoSecrets.ps1 -Provider Foundry -ResourceUrl https://your-resource.services.ai.azure.com/ -UseApiKey

Run off camera. JSON travels through stdin, never secret-valued arguments.
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [ValidateSet('Foundry', 'OpenAI')][string]$Provider,
    [string]$Model,
    [string]$ResourceUrl,
    [switch]$UseApiKey
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Read-MenuChoice {
    param(
        [string]$Prompt,
        [string[]]$Choices,
        [string]$Default
    )
    while ($true) {
        $answer = Read-Host "$Prompt [$Default]"
        if ([string]::IsNullOrWhiteSpace($answer)) { return $Default }
        if ($Choices -ccontains $answer.Trim()) { return $answer.Trim() }
        Write-Host "Enter one of: $($Choices -join ', ')." -ForegroundColor Yellow
    }
}

function Read-TextWithDefault {
    param([string]$Prompt, [string]$Default)
    $answer = Read-Host "$Prompt [$Default]"
    if ([string]::IsNullOrWhiteSpace($answer)) { return $Default }
    return $answer.Trim()
}

function Get-FoundryEndpoint {
    param([Parameter(Mandatory)][string]$Value)
    $parsed = $null
    if (-not [Uri]::TryCreate($Value, [UriKind]::Absolute, [ref]$parsed) -or
        $parsed.Scheme -ne 'https' -or $parsed.UserInfo -or $parsed.Query -or
        $parsed.Fragment -or $parsed.AbsolutePath.TrimEnd('/') -notin @('', '/openai/v1')) {
        throw 'Enter the trusted resource HTTPS origin or its /openai/v1/ endpoint, without project paths or credentials.'
    }
    return $parsed.GetLeftPart([UriPartial]::Authority) + '/openai/v1/'
}

if (-not $PSBoundParameters.ContainsKey('Provider')) {
    Write-Host ''
    Write-Host 'Choose a provider configuration. Only the selected provider User Secrets are updated.'
    Write-Host '  1. Microsoft Foundry, shared by BYOK 01 and examples-app 04 (default)'
    Write-Host '     Resource endpoint plus Entra or a Foundry resource API key'
    Write-Host '  2. OpenAI, used by BYOK 04, direct API key (not Azure or a ChatGPT subscription)'
    $choice = Read-MenuChoice -Prompt 'Provider, default 1' -Choices @('1', '2') -Default '1'
    $Provider = if ($choice -eq '1') { 'Foundry' } else { 'OpenAI' }
}

if ($Provider -eq 'Foundry') {
    $project = Join-Path $PSScriptRoot 'byok\01 - Microsoft Foundry\MicrosoftFoundry.csproj'
    Write-Host 'This Foundry configuration is shared with examples-app\04-comparar-proveedores.'
    if ([string]::IsNullOrWhiteSpace($Model)) {
        $Model = Read-TextWithDefault `
            -Prompt 'Foundry deployment/model ID (availability depends on your resource)' `
            -Default 'gpt-6-luna'
    }
    if ([string]::IsNullOrWhiteSpace($ResourceUrl)) {
        Write-Host 'Use your Azure AI Foundry resource endpoint, not a project URL or Azure portal URL.'
        Write-Host 'Example: https://your-resource.services.ai.azure.com/openai/v1/'
        while ($true) {
            $ResourceUrl = Read-Host 'Foundry resource endpoint (required)'
            try {
                $baseUrl = Get-FoundryEndpoint -Value $ResourceUrl
                break
            }
            catch {
                Write-Host $_.Exception.Message -ForegroundColor Yellow
            }
        }
    }
    else {
        $baseUrl = Get-FoundryEndpoint -Value $ResourceUrl
    }

    if (-not $PSBoundParameters.ContainsKey('Provider')) {
        Write-Host ''
        Write-Host 'Foundry authentication: Entra uses your existing Azure CLI login; it will not sign in.'
        Write-Host 'A resource API key is optional and is different from an Azure subscription key.'
        Write-Host '  1. Microsoft Entra, default'
        Write-Host '  2. Foundry resource API key, entered securely'
        $UseApiKey = (Read-MenuChoice -Prompt 'Authentication, default 1' `
            -Choices @('1', '2') -Default '1') -eq '2'
    }

    $values = @{
        'Foundry:BaseUrl' = $baseUrl
        'Foundry:Model' = $Model
        # Empty explicitly selects Entra, replacing any previous optional key.
        'Foundry:ApiKey' = ''
    }
}
else {
    $project = Join-Path $PSScriptRoot 'byok\04 - OpenAI\OpenAi.csproj'
    if ([string]::IsNullOrWhiteSpace($Model)) {
        Write-Host 'Choose a Responses API model enabled for your OpenAI account.'
        $Model = Read-TextWithDefault -Prompt 'OpenAI model ID' -Default 'gpt-4.1-mini'
    }
    if ([string]::IsNullOrWhiteSpace($Model)) { throw 'Model must not be empty.' }
    $values = @{ 'OpenAI:Model' = $Model }
}

if (-not $PSCmdlet.ShouldProcess($project, "Set $Provider per-project User Secrets")) {
    $values.Clear()
    return
}
if ($Provider -eq 'OpenAI' -or $UseApiKey) {
    if ($Provider -eq 'OpenAI') {
        Write-Host 'Enter an OpenAI API key. Input is hidden; it is never shown or used as an Azure credential.'
    }
    else {
        Write-Host 'Enter the Foundry resource API key. Input is hidden; it is never shown.'
    }
    $secret = Read-Host 'API key (hidden)' -AsSecureString
    $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secret)
    try {
        $key = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer)
        if ([string]::IsNullOrWhiteSpace($key)) { throw 'API key must not be empty.' }
        $values["${Provider}:ApiKey"] = $key
    }
    finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer)
        $secret.Dispose()
        Remove-Variable key -ErrorAction SilentlyContinue
    }
}
try {
    # Piped JSON avoids putting keys in process arguments or PowerShell history.
    # dotnet merges only the supplied keys. It does not list existing values.
    $values | ConvertTo-Json -Compress | & dotnet user-secrets set --project $project
    if ($LASTEXITCODE -ne 0) { throw 'dotnet user-secrets set failed.' }
}
finally {
    $values.Clear()
}
Write-Host "$Provider User Secrets saved outside the repo. No auth, runtime or model call."
