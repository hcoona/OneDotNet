param(
  [string] $Configuration = "Release",
  [switch] $BuildDos2Unix,
  [switch] $SkipGriPlc,
  [switch] $SkipGriServer,
  [switch] $SkipGriWpf
)

$ErrorActionPreference = "Stop"

$EnlistmentRoot = [System.IO.Path]::GetDirectoryName([System.IO.Path]::GetDirectoryName($PSScriptRoot))
$OutputRoot = (Join-Path $EnlistmentRoot "output")
if ([System.IO.Directory]::Exists($OutputRoot)) {
  Remove-Item -Path $OutputRoot -Recurse -Force
}
New-Item -Path $OutputRoot -ItemType Directory

if ($BuildDos2Unix) {
  $DosToUnixRoot = Join-Path $OutputRoot "dos2unix"
  Copy-Item `
    (Join-Path $EnlistmentRoot "third_party/dos2unix-7.4.1") `
    -Destination $DosToUnixRoot `
    -Recurse
  Push-Location $DosToUnixRoot
  nmake.exe /f vc.mak
  Pop-Location
  $UnixToDosExec = Join-Path $DosToUnixRoot "dos2unix.exe"
} else {
  $UnixToDosExec = Join-Path $EnlistmentRoot "dev-support/bin/unix2dos.exe"
}

if (-not $SkipGriServer) {
  dotnet.exe publish `
    -c $Configuration `
    -f "netcoreapp3.1" `
    -o (Join-Path $OutputRoot "GeothermalResearchInstitute/ServerConsole") `
    (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/GeothermalResearchInstitute.ServerConsole")
  if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to publish GeothermalResearchInstitute ServerConsole project."
  }
}

if (-not $SkipGriPlc) {
  dotnet.exe publish `
    -c $Configuration `
    -f "netcoreapp3.1" `
    -o (Join-Path $OutputRoot "GeothermalResearchInstitute/FakePlcV2") `
    (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/GeothermalResearchInstitute.FakePlcV2")
  if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to publish GeothermalResearchInstitute FakePlcV2 project."
  }
}

if (-not $SkipGriWpf) {
  dotnet.exe publish `
    -c $Configuration `
    -f "netcoreapp3.1" `
    -o (Join-Path $OutputRoot "GeothermalResearchInstitute/Wpf") `
    (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/GeothermalResearchInstitute.Wpf")
  if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to publish GeothermalResearchInstitute Wpf project."
  }
}

Get-ChildItem -Path (Join-Path $OutputRoot "GeothermalResearchInstitute") -Filter "*.ini" -Recurse `
  | ForEach-Object{ & "$UnixToDosExec" --verbose --add-bom $_.FullName }

New-Item -Path (Join-Path $OutputRoot "GeothermalResearchInstitute/docs") -Type Directory
Copy-Item -Path (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/docs/*") -Destination (Join-Path $OutputRoot "GeothermalResearchInstitute/docs") -Include "*.pdf", "*.exe", "*.msu" -Recurse
