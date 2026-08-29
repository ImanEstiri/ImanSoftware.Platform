param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectName,

    [switch]$SkipPush
)

$ErrorActionPreference = "Stop"

$solutionRoot      = $PSScriptRoot
$projectPath       = Join-Path $solutionRoot "src\$ProjectName"
$artifactsPath     = Join-Path $solutionRoot "artifacts\$ProjectName"
$srcRoot           = Join-Path $solutionRoot "src"
$packagesPropsPath = Join-Path $solutionRoot "Directory.Packages.props"

# ===================================================================
# Logging helpers
#   Write-Step   -> yellow : status / decision messages
#   Write-Action -> blue   : "about to run a command" announcements
#   (actual command output is left uncolored / default terminal color)
#   Add-Summary  -> records a line for the final summary block
# ===================================================================

$script:summary = @()

function Write-Step {
    param([Parameter(Mandatory = $true)][string]$Message)
    Write-Host $Message -ForegroundColor Yellow
}

function Write-Action {
    param([Parameter(Mandatory = $true)][string]$Message)
    Write-Host $Message -ForegroundColor Blue
}

function Add-Summary {
    param([Parameter(Mandatory = $true)][string]$Message)
    $script:summary += $Message
}

function Show-Summary {
    Write-Host ""
    Write-Host "===================== Summary =====================" -ForegroundColor Cyan
    if ($script:summary.Count -eq 0) {
        Write-Host " (nothing was recorded)" -ForegroundColor Cyan
    } else {
        foreach ($line in $script:summary) {
            Write-Host " - $line" -ForegroundColor Cyan
        }
    }
    Write-Host "=====================================================" -ForegroundColor Cyan
}

function Bump-PatchVersion {
    param([Parameter(Mandatory = $true)][string]$Version)

    if ($Version -notmatch '^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(?<suffix>[-+].*)?$') {
        throw "Version '$Version' is not in a recognizable Major.Minor.Patch format."
    }

    $major  = $Matches['major']
    $minor  = $Matches['minor']
    $patch  = [int]$Matches['patch'] + 1
    $suffix = $Matches['suffix']

    return "$major.$minor.$patch$suffix"
}

function Get-CsprojVersion {
    param([Parameter(Mandatory = $true)][string]$Path)
    [xml]$xml = Get-Content $Path
    return $xml.Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1
}

function Set-CsprojVersion {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$NewVersion
    )
    [xml]$xml = Get-Content $Path
    $propertyGroup = $xml.Project.PropertyGroup | Where-Object { $_.Version } | Select-Object -First 1

    if (-not $propertyGroup) {
        throw "Could not find a <PropertyGroup> with a <Version> element in $Path"
    }

    $propertyGroup.Version = $NewVersion
    $xml.Save($Path)
}

function Test-HasPackageReference {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Name
    )
    [xml]$xml = Get-Content $Path
    $refs = $xml.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq $Name }
    return [bool]$refs
}

# ===================================================================
# Main
# ===================================================================

try {
    if (-not (Test-Path $projectPath)) {
        Write-Error "No project named '$ProjectName' found under src/: $projectPath"
    }

    # ===== API Key =====
    # Recommendation: do NOT hardcode the key in this file. Set it once, outside of git:
    #   setx NUGET_API_KEY "your-real-key-here"
    # Then close and reopen your terminal / Visual Studio so the new value is loaded.
    $ApiKey = $env:NUGET_API_KEY

    if ([string]::IsNullOrWhiteSpace($ApiKey)) {
        Write-Error "Environment variable NUGET_API_KEY is not set. Run 'setx NUGET_API_KEY your-key' and restart your terminal."
    }

    # Note: previous builds are kept intentionally, so you have a local history
    # of every version ever packed. We don't delete anything here.
    if (-not (Test-Path $artifactsPath)) {
        Write-Step "artifacts folder for $ProjectName does not exist yet; creating $artifactsPath"
        New-Item -Path $artifactsPath -ItemType Directory | Out-Null
    }

    $csprojPath = Join-Path $projectPath "$ProjectName.csproj"

    if (-not (Test-Path $csprojPath)) {
        Write-Error "Could not find project file: $csprojPath"
    }

    # ---------------------------------------------------------------
    # Part 1: pack (auto-bumping the patch version if it was already packed)
    # ---------------------------------------------------------------

    $version = Get-CsprojVersion -Path $csprojPath

    if (-not $version) {
        Write-Error "Could not read <Version> from $csprojPath. Make sure it's set explicitly in the project file."
    }

    $originalVersion = $version

    Write-Step "checking artifact/$ProjectName for any $version output."
    $expectedNupkg = Join-Path $artifactsPath "$ProjectName.$version.nupkg"

    while (Test-Path $expectedNupkg) {
        $previous = $version
        $version  = Bump-PatchVersion -Version $version
        Write-Step "auto bumping version for $ProjectName from $previous to $version"
        Add-Summary "Version auto-bumped: $previous -> $version (an artifact for $previous already existed)"
        Write-Step "checking artifact/$ProjectName for any $version output."
        $expectedNupkg = Join-Path $artifactsPath "$ProjectName.$version.nupkg"
    }

    Write-Step "version $version is free; no existing artifact found."

    if ($version -ne $originalVersion) {
        Set-CsprojVersion -Path $csprojPath -NewVersion $version
        Write-Step "updated <Version> in $csprojPath : $originalVersion -> $version"
        Add-Summary "csproj updated: $ProjectName Version $originalVersion -> $version"
    } else {
        Add-Summary "Version used: $ProjectName $version (no bump needed)"
    }

    Write-Action "packing $ProjectName v$version ..."
    dotnet pack $projectPath -c Release --output $artifactsPath
    if ($LASTEXITCODE -ne 0) {
        Add-Summary "FAILED: dotnet pack for $ProjectName v$version"
        Write-Error "Pack failed."
    }
    Add-Summary "Packed: $ProjectName v$version -> $artifactsPath"

    # Multiple versions may already sit in this folder from previous runs.
    # The one we just built is always the most recently modified file, so we
    # sort by LastWriteTime and take the newest — nothing else gets touched.
    $nupkg = Get-ChildItem -Path $artifactsPath -Filter "*.nupkg" |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if (-not $nupkg) {
        Write-Error "No .nupkg file was found after packing."
    }

    $snupkgPath = Join-Path $artifactsPath "$ProjectName.$version.snupkg"

    Write-Step "package built: $($nupkg.Name)"

    if ($SkipPush) {
        Write-Step "SkipPush is set; packed only, not pushed. Directory.Packages.props and dependent projects were NOT updated."
        Add-Summary "Push skipped (-SkipPush): $($nupkg.Name) left in $artifactsPath"
        return
    }

    # ---------------------------------------------------------------
    # Push (15s timeout; on failure, remove the artifacts we just built
    # so the next run doesn't see a "used" version and bump for no reason)
    # ---------------------------------------------------------------

    Write-Action "pushing package to nuget ..."
    dotnet nuget push $nupkg.FullName `
        --source https://api.nuget.org/v3/index.json `
        --api-key $ApiKey `
        --skip-duplicate `
        --timeout 15

    if ($LASTEXITCODE -ne 0) {
        Write-Step "push failed or timed out (15s); removing the artifacts just built for v$version so the version isn't wasted."

        if (Test-Path $nupkg.FullName) {
            Remove-Item $nupkg.FullName -Force
            Write-Step "removed $($nupkg.Name)"
            Add-Summary "Removed after failed push: $($nupkg.Name)"
        }
        if (Test-Path $snupkgPath) {
            Remove-Item $snupkgPath -Force
            Write-Step "removed $(Split-Path $snupkgPath -Leaf)"
            Add-Summary "Removed after failed push: $(Split-Path $snupkgPath -Leaf)"
        }

        Add-Summary "FAILED: dotnet nuget push for $ProjectName v$version"
        Write-Error "Push failed."
    }

    Write-Step "$($nupkg.Name) was successfully published to NuGet.org."
    Add-Summary "Pushed to NuGet.org: $($nupkg.Name)"

    # ---------------------------------------------------------------
    # Part 2: sync Directory.Packages.props with the version we just pushed
    # ---------------------------------------------------------------

    Write-Step "checking Directory.Packages.props for an existing $ProjectName entry."

    if (-not (Test-Path $packagesPropsPath)) {
        Write-Error "Directory.Packages.props not found at $packagesPropsPath"
    }

    [xml]$propsXml = Get-Content $packagesPropsPath

    $existingEntry = $propsXml.Project.ItemGroup.PackageVersion |
        Where-Object { $_.Include -eq $ProjectName } |
        Select-Object -First 1

    if ($existingEntry) {
        $oldPropsVersion = $existingEntry.Version
        if ($oldPropsVersion -eq $version) {
            Write-Step "Directory.Packages.props already has $ProjectName = $version; nothing to change."
            Add-Summary "Directory.Packages.props: $ProjectName already at $version"
        } else {
            $existingEntry.Version = $version
            $propsXml.Save($packagesPropsPath)
            Write-Step "updating $ProjectName in Directory.Packages.props from $oldPropsVersion to $version"
            Add-Summary "Directory.Packages.props updated: $ProjectName $oldPropsVersion -> $version"
        }
    } else {
        Write-Step "$ProjectName not found in Directory.Packages.props; adding a new entry."
        $itemGroup = $propsXml.Project.ItemGroup | Select-Object -First 1
        if (-not $itemGroup) {
            $itemGroup = $propsXml.CreateElement("ItemGroup")
            $propsXml.Project.AppendChild($itemGroup) | Out-Null
        }

        $newEntry = $propsXml.CreateElement("PackageVersion")
        $newEntry.SetAttribute("Include", $ProjectName)
        $newEntry.SetAttribute("Version", $version)
        $itemGroup.AppendChild($newEntry) | Out-Null

        $propsXml.Save($packagesPropsPath)
        Write-Step "added new entry to Directory.Packages.props: $ProjectName = $version"
        Add-Summary "Directory.Packages.props: added new entry $ProjectName = $version"
    }

    # ---------------------------------------------------------------
    # Part 3: bump the patch version of every project under ./src that
    # depends on $ProjectName. They are NOT packed/pushed here.
    # ---------------------------------------------------------------

    Write-Step "scanning $srcRoot for projects that reference $ProjectName ..."

    $candidateCsprojFiles = Get-ChildItem -Path $srcRoot -Filter "*.csproj" -Recurse |
        Where-Object { $_.FullName -ne $csprojPath }   # skip the package's own csproj

    $bumpedProjects = @()

    foreach ($file in $candidateCsprojFiles) {
        if (-not (Test-HasPackageReference -Path $file.FullName -Name $ProjectName)) {
            continue
        }

        $depVersion = Get-CsprojVersion -Path $file.FullName
        if (-not $depVersion) {
            Write-Step "skipping $($file.Name): references $ProjectName but has no <Version> to bump."
            Add-Summary "Skipped (no <Version>): $($file.Name)"
            continue
        }

        $newDepVersion = Bump-PatchVersion -Version $depVersion
        Set-CsprojVersion -Path $file.FullName -NewVersion $newDepVersion
        $bumpedProjects += [PSCustomObject]@{ Name = $file.BaseName; Old = $depVersion; New = $newDepVersion }
        Write-Step "bumping dependent project $($file.BaseName) from $depVersion to $newDepVersion"
        Add-Summary "Dependent project bumped: $($file.BaseName) $depVersion -> $newDepVersion"
    }

    if ($bumpedProjects.Count -eq 0) {
        Write-Step "no dependent projects found under $srcRoot."
        Add-Summary "No dependent projects found under src/ referencing $ProjectName"
    } else {
        Write-Step "bumped patch version for $($bumpedProjects.Count) dependent project(s)."
        Write-Step "NOTE: these projects were only version-bumped, not packed or pushed."
        Write-Step "NuGet.org indexing can take a few minutes. Wait until $ProjectName v$version"
        Write-Step "is resolvable (dotnet restore succeeds) before running .\publish.ps1 for them:"
        foreach ($p in $bumpedProjects) {
            Write-Step "    .\publish.ps1 -ProjectName $($p.Name)"
        }
    }
}
finally {
    Show-Summary
}