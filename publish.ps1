param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectName,

    [switch]$SkipPush
)

$ErrorActionPreference = "Stop"

$solutionRoot  = $PSScriptRoot
$projectPath   = Join-Path $solutionRoot "src\$ProjectName"
$artifactsPath = Join-Path $solutionRoot "artifacts\$ProjectName"

if (-not (Test-Path $projectPath)) {
    Write-Error "No project named '$ProjectName' found under src/: $projectPath"
    exit 1
}

# ===== API Key =====
# Recommendation: do NOT hardcode the key in this file. Set it once, outside of git:
#   setx NUGET_API_KEY "your-real-key-here"
# Then close and reopen your terminal / Visual Studio so the new value is loaded.
$ApiKey = $env:NUGET_API_KEY

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    Write-Error "Environment variable NUGET_API_KEY is not set. Run 'setx NUGET_API_KEY your-key' and restart your terminal."
    exit 1
}

# Note: previous builds are kept intentionally, so you have a local history
# of every version ever packed. We don't delete anything here.
if (-not (Test-Path $artifactsPath)) {
    New-Item -Path $artifactsPath -ItemType Directory | Out-Null
}

# ===== Guard: refuse to overwrite an already-built version =====
# dotnet pack would silently overwrite an existing .nupkg with the same
# name, which quietly destroys part of your local version history.
# We read <Version> straight from the csproj and check ahead of time.
$csprojPath = Join-Path $projectPath "$ProjectName.csproj"

if (-not (Test-Path $csprojPath)) {
    Write-Error "Could not find project file: $csprojPath"
    exit 1
}

[xml]$csprojXml = Get-Content $csprojPath
$version = $csprojXml.Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1

if (-not $version) {
    Write-Error "Could not read <Version> from $csprojPath. Make sure it's set explicitly in the project file."
    exit 1
}

$expectedNupkg = Join-Path $artifactsPath "$ProjectName.$version.nupkg"

if (Test-Path $expectedNupkg) {
    Write-Error "Version $version of $ProjectName was already packed:`n  $expectedNupkg`nBump <Version> in $csprojPath before packing again."
    exit 1
}

Write-Host "==> Packing $ProjectName v$version ..." -ForegroundColor Cyan
dotnet pack $projectPath -c Release --output $artifactsPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Pack failed."
    exit 1
}

# Multiple versions may already sit in this folder from previous runs.
# The one we just built is always the most recently modified file, so we
# sort by LastWriteTime and take the newest — nothing else gets touched.
$nupkg = Get-ChildItem -Path $artifactsPath -Filter "*.nupkg" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $nupkg) {
    Write-Error "No .nupkg file was found after packing."
    exit 1
}

Write-Host "==> Package built: $($nupkg.Name)" -ForegroundColor Green

if ($SkipPush) {
    Write-Host "==> SkipPush is set; packed only, not pushed." -ForegroundColor Yellow
    exit 0
}

Write-Host "==> Pushing to NuGet.org ..." -ForegroundColor Cyan
dotnet nuget push $nupkg.FullName `
    --source https://api.nuget.org/v3/index.json `
    --api-key $ApiKey `
    --skip-duplicate

if ($LASTEXITCODE -ne 0) {
    Write-Error "Push failed."
    exit 1
}

Write-Host "==> $($nupkg.Name) was successfully published to NuGet.org." -ForegroundColor Green