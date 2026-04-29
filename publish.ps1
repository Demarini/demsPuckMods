<#
.SYNOPSIS
    Steam Workshop publishing tool for Puck mods.

.EXAMPLE
    .\publish.ps1 list
    .\publish.ps1 ModifyMinimapIcons "Fixed icon scaling"
    .\publish.ps1 all "Compatibility update for v1.2"
    .\publish.ps1 toggle BanIdiots
    .\publish.ps1 clean
    .\publish.ps1 -NoBuild ModifyMinimapIcons "Hotfix"
#>
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$Command,

    [Parameter(Position = 1)]
    [string]$Arg,

    [switch]$NoBuild
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ConfigPath = Join-Path $ScriptDir "workshop.json"

if (-not (Test-Path $ConfigPath)) {
    Write-Host "  workshop.json not found at $ConfigPath" -ForegroundColor Red
    exit 1
}

$config = Get-Content $ConfigPath -Raw | ConvertFrom-Json

# ---------------------------------------------------------------------------

function Show-Usage {
    Write-Host @"

  publish.ps1 - Steam Workshop publishing tool for Puck mods

  Commands:
    .\publish.ps1 <ModName> "changenotes"    Publish a single mod
    .\publish.ps1 all "changenotes"           Publish all active mods
    .\publish.ps1 list                        Show mods and their status
    .\publish.ps1 toggle <ModName>            Toggle whether a mod is included in 'all'
    .\publish.ps1 clean [ModName]             Remove mod(s) from the game Plugins folder

  Flags:
    -NoBuild                                  Skip the build step (use existing staging output)

  Building via this script deploys to the staging folder ($($config.staging_dir))
  instead of your game's Plugins directory, so local testing copies are not affected.

"@
}

function Build-Mod {
    param([string]$ModName, $Mod)

    $csproj = Join-Path $ScriptDir $Mod.csproj
    if (-not (Test-Path $csproj)) {
        Write-Host "  csproj not found: $csproj" -ForegroundColor Red
        return $false
    }

    $stagingDir = $config.staging_dir
    Write-Host "  Building $ModName..." -ForegroundColor Cyan

    dotnet build $csproj -c Release /p:PuckInstallDir="$stagingDir" -v minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Build FAILED for $ModName" -ForegroundColor Red
        return $false
    }

    Write-Host "  Build OK" -ForegroundColor Green
    return $true
}

function Publish-Mod {
    param([string]$ModName, $Mod, [string]$ChangeNote)

    if (-not $Mod.publishedfileid -or $Mod.publishedfileid -eq "") {
        Write-Host "  Skipping $ModName - no publishedfileid in workshop.json" -ForegroundColor Yellow
        return
    }

    $contentFolder = Join-Path (Join-Path $config.staging_dir "Plugins") $Mod.plugin_folder

    if (-not (Test-Path $contentFolder)) {
        Write-Host "  Content folder not found: $contentFolder" -ForegroundColor Red
        Write-Host "  Did the build succeed? Check plugin_folder in workshop.json." -ForegroundColor Red
        return
    }

    # Write a temporary VDF for this mod
    $vdfPath = Join-Path $config.steamcmd_path "update_$ModName.vdf"

    $vdfContent = @"
"workshopitem"
{
    "appid" "$($config.app_id)"
    "publishedfileid" "$($Mod.publishedfileid)"
    "contentfolder" "$contentFolder"
    "changenote" "$ChangeNote"
}
"@
    $vdfContent | Out-File -FilePath $vdfPath -Encoding ascii

    Write-Host ""
    Write-Host "  Publishing $ModName" -ForegroundColor Green
    Write-Host "    Workshop ID : $($Mod.publishedfileid)" -ForegroundColor DarkGray
    Write-Host "    Content     : $contentFolder" -ForegroundColor DarkGray
    Write-Host "    Changenote  : $ChangeNote" -ForegroundColor DarkGray

    $steamcmd = Join-Path $config.steamcmd_path "steamcmd.exe"
    & $steamcmd +login $config.steam_login +workshop_build_item $vdfPath +quit

    if ($LASTEXITCODE -ne 0) {
        Write-Host "  SteamCMD returned an error for $ModName" -ForegroundColor Red
    }
    else {
        Write-Host "  Upload complete for $ModName" -ForegroundColor Green
    }

    Remove-Item $vdfPath -ErrorAction SilentlyContinue
}

# ---------------------------------------------------------------------------
#  Main
# ---------------------------------------------------------------------------

if (-not $Command) {
    Show-Usage
    exit 0
}

switch ($Command) {

    "list" {
        Write-Host ""
        Write-Host "  Steam Workshop Mods" -ForegroundColor Cyan
        Write-Host ("  " + ("-" * 65))
        $fmt = "  {0,-10} {1,-25} {2}"
        Write-Host ($fmt -f "Status", "Name", "Workshop ID") -ForegroundColor DarkGray
        Write-Host ("  " + ("-" * 65))

        foreach ($prop in $config.mods.PSObject.Properties) {
            $active = $prop.Value.active
            $status = if ($active) { "[active]" } else { "[      ]" }
            $color  = if ($active) { "Green"   } else { "Gray"    }
            $id = if ($prop.Value.publishedfileid -and $prop.Value.publishedfileid -ne "") {
                $prop.Value.publishedfileid
            }
            else { "(not set)" }
            Write-Host ($fmt -f $status, $prop.Name, $id) -ForegroundColor $color
        }
        Write-Host ""
    }

    "toggle" {
        if (-not $Arg) {
            Write-Host "  Usage: .\publish.ps1 toggle <ModName>" -ForegroundColor Yellow
            exit 1
        }

        $json = Get-Content $ConfigPath -Raw | ConvertFrom-Json
        $target = $json.mods.PSObject.Properties | Where-Object { $_.Name -eq $Arg }

        if (-not $target) {
            Write-Host "  Unknown mod: $Arg" -ForegroundColor Red
            Write-Host "  Run '.\publish.ps1 list' to see available mods." -ForegroundColor Yellow
            exit 1
        }

        $newState = -not $target.Value.active
        $target.Value.active = $newState
        $json | ConvertTo-Json -Depth 5 | Out-File $ConfigPath -Encoding utf8

        $label = if ($newState) { "ACTIVE" } else { "INACTIVE" }
        Write-Host "  $Arg is now $label" -ForegroundColor Green
    }

    "clean" {
        $pluginsDir = Join-Path $config.puck_install_dir "Plugins"

        if ($Arg) {
            $target = $config.mods.PSObject.Properties | Where-Object { $_.Name -eq $Arg }
            if (-not $target) {
                Write-Host "  Unknown mod: $Arg" -ForegroundColor Red
                exit 1
            }
            $folder = Join-Path $pluginsDir $target.Value.plugin_folder
            if (Test-Path $folder) {
                Remove-Item $folder -Recurse -Force -Confirm:$false
                Write-Host "  Removed $($target.Value.plugin_folder)" -ForegroundColor Yellow
            }
            else {
                Write-Host "  $($target.Value.plugin_folder) not found in Plugins" -ForegroundColor DarkGray
            }
        }
        else {
            Write-Host ""
            Write-Host "  Cleaning locally-deployed mods from $pluginsDir ..." -ForegroundColor Cyan
            $removed = 0
            foreach ($prop in $config.mods.PSObject.Properties) {
                $folder = Join-Path $pluginsDir $prop.Value.plugin_folder
                if (Test-Path $folder) {
                    Remove-Item $folder -Recurse -Force -Confirm:$false
                    Write-Host "  Removed $($prop.Value.plugin_folder)" -ForegroundColor Yellow
                    $removed++
                }
            }
            if ($removed -eq 0) {
                Write-Host "  Nothing to clean." -ForegroundColor DarkGray
            }
            else {
                Write-Host "  Cleaned $removed mod folder(s)." -ForegroundColor Green
            }
        }
    }

    "all" {
        $changeNote = $Arg
        if (-not $changeNote) {
            $changeNote = Read-Host "  Enter changenotes for all active mods"
        }

        $activeMods = @($config.mods.PSObject.Properties | Where-Object { $_.Value.active })

        if ($activeMods.Count -eq 0) {
            Write-Host "  No active mods. Use '.\publish.ps1 toggle <ModName>' to activate." -ForegroundColor Yellow
            exit 0
        }

        Write-Host ""
        Write-Host "  Publishing $($activeMods.Count) active mod(s)..." -ForegroundColor Cyan
        Write-Host ("  " + ("=" * 50))

        foreach ($prop in $activeMods) {
            Write-Host ""
            Write-Host ("  --- $($prop.Name) ---") -ForegroundColor White

            if (-not $NoBuild) {
                $built = Build-Mod -ModName $prop.Name -Mod $prop.Value
                if (-not $built) {
                    Write-Host "  Skipping publish due to build failure" -ForegroundColor Red
                    continue
                }
            }

            Publish-Mod -ModName $prop.Name -Mod $prop.Value -ChangeNote $changeNote
        }

        Write-Host ""
        Write-Host ("  " + ("=" * 50))
        Write-Host "  All done." -ForegroundColor Green
        Write-Host ""
    }

    default {
        # Treat $Command as a mod name
        $modName = $Command
        $changeNote = $Arg

        $target = $config.mods.PSObject.Properties | Where-Object { $_.Name -eq $modName }

        if (-not $target) {
            Write-Host "  Unknown mod: $modName" -ForegroundColor Red
            Write-Host "  Run '.\publish.ps1 list' to see available mods." -ForegroundColor Yellow
            exit 1
        }

        if (-not $changeNote) {
            $changeNote = Read-Host "  Enter changenotes for $modName"
        }

        if (-not $NoBuild) {
            $built = Build-Mod -ModName $modName -Mod $target.Value
            if (-not $built) { exit 1 }
        }

        Publish-Mod -ModName $modName -Mod $target.Value -ChangeNote $changeNote
        Write-Host ""
    }
}
