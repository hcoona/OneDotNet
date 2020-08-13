param(
  [string] $Configuration = "Release",
  [switch] $SkipUnix2Dos,
  [switch] $SkipGriPlc,
  [switch] $SkipGriServer,
  [switch] $SkipGriWpf
)

$ErrorActionPreference = "Stop"

$EnlistmentRoot = [System.IO.Path]::GetDirectoryName([System.IO.Path]::GetDirectoryName($PSScriptRoot))
$OutputRoot = (Join-Path $EnlistmentRoot "output")
$UnixToDosExec = (Join-Path $EnlistmentRoot "dev-support\bin\vs2019_dos2unix\unix2dos.exe")

if ([System.IO.Directory]::Exists($OutputRoot)) {
  Remove-Item -Path $OutputRoot -Recurse -Force
}
New-Item -Path $OutputRoot -ItemType Directory

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

if (-not $SkipUnix2Dos) {
  Get-ChildItem -Path (Join-Path $OutputRoot "GeothermalResearchInstitute") -Filter "*.ini" -Recurse `
    | ForEach-Object{ & $UnixToDosExec --add-bom $_.FullName }
}

New-Item -Path (Join-Path $OutputRoot "GeothermalResearchInstitute/docs") -Type Directory
Copy-Item -Path (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/docs/*") -Destination (Join-Path $OutputRoot "GeothermalResearchInstitute/docs") -Include "*.pdf", "*.exe", "*.msu" -Recurse
