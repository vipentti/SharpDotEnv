[CmdletBinding()]
param (
    [Parameter()] [string] $EnvFile = ".env",
    [Parameter()] [switch] $Override
)

$root = $PSScriptRoot
$dllPath = ".\src\SharpDotEnv\bin\Debug\net7.0\SharpDotEnv.dll"

if (-not (Test-Path $dllPath)) {
    & "$root\build.ps1" Compile --no-logo
}

Write-Host "Root: $root"

$job = Start-Job -ScriptBlock {
    Add-Type -Path $args[0]

    $options = New-Object -TypeName SharpDotEnv.DotEnvConfigOptions -Property @{
        Path = $args[1]
    }

    [SharpDotEnv.DotEnv]::Parse($options)
} -ArgumentList $dllPath, $EnvFile
Wait-Job $job | Out-Null

$dotenv = Receive-Job $job

foreach ($kvp in $dotenv.GetEnumerator()) {
    $key = $kvp.Key
    $val = $kvp.Value

    if (-not (Test-Path "env:$key") -or $Override) {
        Write-Host "Setting: $key"
        Set-Item -Path "env:$key" -Value "$val" | Out-Null
    }
}

