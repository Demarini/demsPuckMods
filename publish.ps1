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
    .\publish.ps1 map list
    .\publish.ps1 map DanceClub "Initial release"
    .\publish.ps1 map all "Texture pass"
    .\publish.ps1 map toggle DanceClub
#>
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$Command,

    [Parameter(Position = 1)]
    [string]$Arg,

    [Parameter(Position = 2)]
    [string]$Arg2,

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

  Mod commands:
    .\publish.ps1 <ModName> "changenotes"    Publish a single mod
    .\publish.ps1 all "changenotes"           Publish all active mods
    .\publish.ps1 list                        Show mods and their status
    .\publish.ps1 toggle <ModName>            Toggle whether a mod is included in 'all'
    .\publish.ps1 clean [ModName]             Remove mod(s) from the game Plugins folder

  Scenery map commands (each map is its own Workshop item, no build step):
    .\publish.ps1 map list                    Show scenery maps and their status
    .\publish.ps1 map <Name> "changenotes"   Publish/create a single scenery map
    .\publish.ps1 map all "changenotes"       Publish all active scenery maps
    .\publish.ps1 map toggle <Name>           Toggle whether a map is included in 'map all'

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
#  Scenery map publishing
# ---------------------------------------------------------------------------

function Escape-VdfValue {
    param([string]$Value)
    if (-not $Value) { return "" }
    # Only escape double quotes; single backslashes are tolerated by Steam's
    # VDF parser inside quoted strings (matches what Publish-Mod does for paths).
    return $Value -replace '"', '\"'
}

function Save-WorkshopConfig {
    param($JsonObject)
    $JsonObject | ConvertTo-Json -Depth 8 | Out-File $ConfigPath -Encoding utf8
}

function Get-MapEntry {
    param([string]$MapName)
    if (-not $config.scenery_maps) { return $null }
    return $config.scenery_maps.PSObject.Properties | Where-Object { $_.Name -eq $MapName }
}

function Get-MapsRoot {
    if ($config.scenery_maps_root) {
        return (Join-Path $ScriptDir $config.scenery_maps_root)
    }
    return (Join-Path $ScriptDir "demsPuckMods\SceneryMaps")
}

function Publish-Map {
    param([string]$MapName, $Map, [string]$ChangeNote)

    $contentFolder = Join-Path $ScriptDir $Map.source_dir
    if (-not (Test-Path $contentFolder)) {
        Write-Host "  Source folder not found: $contentFolder" -ForegroundColor Red
        return
    }
    $abFolder = Join-Path $contentFolder "AssetBundles"
    if (-not (Test-Path $abFolder)) {
        Write-Host "  Expected AssetBundles\ subfolder not found in: $contentFolder" -ForegroundColor Red
        Write-Host "  SceneryLoader expects bundles under an AssetBundles\ folder." -ForegroundColor Red
        return
    }

    $isCreate = (-not $Map.publishedfileid) -or ($Map.publishedfileid -eq "")

    $vdfPath = Join-Path $config.steamcmd_path "map_$MapName.vdf"

    if ($isCreate) {
        $title = if ($Map.title) { $Map.title } else { $MapName }
        $desc  = if ($Map.description) { $Map.description } else { "" }
        $vdfContent = @"
"workshopitem"
{
    "appid" "$($config.app_id)"
    "contentfolder" "$contentFolder"
    "title" "$(Escape-VdfValue $title)"
    "description" "$(Escape-VdfValue $desc)"
    "visibility" "2"
    "changenote" "$(Escape-VdfValue $ChangeNote)"
}
"@
    }
    else {
        $vdfContent = @"
"workshopitem"
{
    "appid" "$($config.app_id)"
    "publishedfileid" "$($Map.publishedfileid)"
    "contentfolder" "$contentFolder"
    "changenote" "$(Escape-VdfValue $ChangeNote)"
}
"@
    }

    $vdfContent | Out-File -FilePath $vdfPath -Encoding ascii

    Write-Host ""
    if ($isCreate) {
        Write-Host "  Creating new Workshop item for map $MapName" -ForegroundColor Green
        Write-Host "    Title       : $title" -ForegroundColor DarkGray
        Write-Host "    Visibility  : private (2)  -- flip to public via Workshop page after testing" -ForegroundColor DarkGray
    }
    else {
        Write-Host "  Publishing map $MapName" -ForegroundColor Green
        Write-Host "    Workshop ID : $($Map.publishedfileid)" -ForegroundColor DarkGray
    }
    Write-Host "    Content     : $contentFolder" -ForegroundColor DarkGray
    Write-Host "    Changenote  : $ChangeNote" -ForegroundColor DarkGray

    $steamcmd = Join-Path $config.steamcmd_path "steamcmd.exe"
    $logPath = Join-Path $env:TEMP "steamcmd_map_$MapName.log"

    # Tee output so we can both stream to the user and parse for the new id
    & $steamcmd +login $config.steam_login +workshop_build_item $vdfPath +quit | Tee-Object -FilePath $logPath
    $steamcmdExit = $LASTEXITCODE

    if ($steamcmdExit -ne 0) {
        Write-Host "  SteamCMD returned an error for map $MapName" -ForegroundColor Red
    }

    if ($isCreate) {
        $logContent = ""
        if (Test-Path $logPath) {
            $logContent = Get-Content $logPath -Raw
        }
        # Match either "PublishedFileId : <id>", "PublishedFileId: <id>", "PublishedFileID : <id>",
        # or "Successfully created item: <id>"
        $match = [regex]::Match($logContent, '(?im)(?:PublishedFileI[Dd]\s*[:=]?\s*|created\s+item\s*[:=]?\s*)(\d{6,})')
        if ($match.Success) {
            $newId = $match.Groups[1].Value
            Write-Host ""
            Write-Host "  Captured new PublishedFileId: $newId" -ForegroundColor Green

            # Re-read config to avoid clobbering any concurrent edits, then save
            $json = Get-Content $ConfigPath -Raw | ConvertFrom-Json
            $entry = $json.scenery_maps.PSObject.Properties | Where-Object { $_.Name -eq $MapName }
            if ($entry) {
                $entry.Value.publishedfileid = $newId
                Save-WorkshopConfig -JsonObject $json
                # Update in-memory copy too
                $Map.publishedfileid = $newId
                Write-Host "  Wrote id back to workshop.json" -ForegroundColor Green
            }
            Write-Host ""
            Write-Host "  Next steps:" -ForegroundColor Cyan
            Write-Host "    1. Open https://steamcommunity.com/sharedfiles/filedetails/?id=$newId" -ForegroundColor DarkGray
            Write-Host "    2. Add preview image, tags, and longer description" -ForegroundColor DarkGray
            Write-Host "    3. Change visibility from Private to Public/Unlisted when ready" -ForegroundColor DarkGray
        }
        else {
            Write-Host ""
            Write-Host "  Could not parse a new PublishedFileId from SteamCMD output." -ForegroundColor Yellow
            Write-Host "  Full log: $logPath" -ForegroundColor Yellow
            Write-Host "  Find the new ID on your Workshop page and paste it into workshop.json manually." -ForegroundColor Yellow
        }
    }
    elseif ($steamcmdExit -eq 0) {
        Write-Host "  Upload complete for map $MapName" -ForegroundColor Green
    }

    Remove-Item $vdfPath -ErrorAction SilentlyContinue
}

function Invoke-MapCommand {
    param([string]$SubCommand, [string]$SubArg, [string]$SubArg2)

    switch ($SubCommand) {
        "" {
            Write-Host "  Usage: .\publish.ps1 map <list|toggle|all|<Name>> [arg]" -ForegroundColor Yellow
        }

        "list" {
            $mapsRoot = Get-MapsRoot
            Write-Host ""
            Write-Host "  Scenery Maps" -ForegroundColor Cyan
            Write-Host ("  " + ("-" * 80))
            $fmt = "  {0,-10} {1,-20} {2,-14} {3}"
            Write-Host ($fmt -f "Status", "Name", "Workshop ID", "Source") -ForegroundColor DarkGray
            Write-Host ("  " + ("-" * 80))

            $registered = @{}
            if ($config.scenery_maps) {
                foreach ($prop in $config.scenery_maps.PSObject.Properties) {
                    $active = $prop.Value.active
                    $status = if ($active) { "[active]" } else { "[      ]" }
                    $color  = if ($active) { "Green"   } else { "Gray"    }
                    $id = if ($prop.Value.publishedfileid -and $prop.Value.publishedfileid -ne "") {
                        $prop.Value.publishedfileid
                    } else { "(new)" }
                    $src = $prop.Value.source_dir
                    $srcFull = Join-Path $ScriptDir $src
                    $missing = if (-not (Test-Path $srcFull)) { " !!MISSING!!" } else { "" }
                    Write-Host ($fmt -f $status, $prop.Name, $id, ($src + $missing)) -ForegroundColor $color
                    $registered[$prop.Name] = $true
                }
            }

            # Orphans: folders under SceneryMaps/ not yet in workshop.json
            if (Test-Path $mapsRoot) {
                $orphans = @()
                foreach ($dir in Get-ChildItem -Path $mapsRoot -Directory -ErrorAction SilentlyContinue) {
                    if (-not $registered.ContainsKey($dir.Name)) {
                        $orphans += $dir.Name
                    }
                }
                if ($orphans.Count -gt 0) {
                    Write-Host ("  " + ("-" * 80))
                    Write-Host "  Unregistered folders in ${mapsRoot}:" -ForegroundColor Yellow
                    foreach ($o in $orphans) {
                        Write-Host "    $o" -ForegroundColor Yellow
                    }
                    Write-Host "  Add an entry to scenery_maps in workshop.json to register them." -ForegroundColor DarkGray
                }
            }
            Write-Host ""
        }

        "toggle" {
            if (-not $SubArg) {
                Write-Host "  Usage: .\publish.ps1 map toggle <Name>" -ForegroundColor Yellow
                return
            }
            $json = Get-Content $ConfigPath -Raw | ConvertFrom-Json
            if (-not $json.scenery_maps) {
                Write-Host "  No scenery_maps section in workshop.json" -ForegroundColor Red
                return
            }
            $target = $json.scenery_maps.PSObject.Properties | Where-Object { $_.Name -eq $SubArg }
            if (-not $target) {
                Write-Host "  Unknown map: $SubArg" -ForegroundColor Red
                Write-Host "  Run '.\publish.ps1 map list' to see available maps." -ForegroundColor Yellow
                return
            }
            $newState = -not $target.Value.active
            $target.Value.active = $newState
            Save-WorkshopConfig -JsonObject $json
            $label = if ($newState) { "ACTIVE" } else { "INACTIVE" }
            Write-Host "  Map $SubArg is now $label" -ForegroundColor Green
        }

        "all" {
            $changeNote = $SubArg
            if (-not $changeNote) {
                $changeNote = Read-Host "  Enter changenotes for all active maps"
            }
            if (-not $config.scenery_maps) {
                Write-Host "  No scenery_maps section in workshop.json" -ForegroundColor Yellow
                return
            }
            $activeMaps = @($config.scenery_maps.PSObject.Properties | Where-Object { $_.Value.active })
            if ($activeMaps.Count -eq 0) {
                Write-Host "  No active maps. Use '.\publish.ps1 map toggle <Name>' to activate." -ForegroundColor Yellow
                return
            }

            Write-Host ""
            Write-Host "  Publishing $($activeMaps.Count) active map(s)..." -ForegroundColor Cyan
            Write-Host ("  " + ("=" * 50))
            foreach ($prop in $activeMaps) {
                Write-Host ""
                Write-Host ("  --- $($prop.Name) ---") -ForegroundColor White
                Publish-Map -MapName $prop.Name -Map $prop.Value -ChangeNote $changeNote
            }
            Write-Host ""
            Write-Host ("  " + ("=" * 50))
            Write-Host "  All maps done." -ForegroundColor Green
            Write-Host ""
        }

        default {
            $mapName = $SubCommand
            $changeNote = $SubArg
            $target = Get-MapEntry -MapName $mapName
            if (-not $target) {
                Write-Host "  Unknown map: $mapName" -ForegroundColor Red
                Write-Host "  Run '.\publish.ps1 map list' to see available maps." -ForegroundColor Yellow
                return
            }
            if (-not $changeNote) {
                $changeNote = Read-Host "  Enter changenotes for map $mapName"
            }
            Publish-Map -MapName $mapName -Map $target.Value -ChangeNote $changeNote
            Write-Host ""
        }
    }
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

    "map" {
        Invoke-MapCommand -SubCommand $Arg -SubArg $Arg2 -SubArg2 ""
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
