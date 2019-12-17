$ErrorActionPreference = "Stop"

$EnlistmentRoot = [System.IO.Path]::GetDirectoryName([System.IO.Path]::GetDirectoryName($PSScriptRoot))
$UnixToDosExec = Join-Path $EnlistmentRoot "dev-support/bin/unix2dos.exe"

$OutputRoot = (Join-Path $EnlistmentRoot "output")
if ([System.IO.Directory]::Exists($OutputRoot)) {
  Remove-Item -Path $OutputRoot -Recurse -Force
}
New-Item -Path $OutputRoot -ItemType Directory

dotnet.exe publish -c "Release" -f "netcoreapp3.1" -o (Join-Path $OutputRoot "GeothermalResearchInstitute/ServerConsole") (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/GeothermalResearchInstitute.ServerConsole")
if ($LASTEXITCODE -ne 0) {
  Write-Error "Failed to publish GeothermalResearchInstitute ServerConsole project."
}

dotnet.exe publish -c "Release" -f "netcoreapp3.1" -o (Join-Path $OutputRoot "GeothermalResearchInstitute/FakePlcV2") (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/GeothermalResearchInstitute.FakePlcV2")
if ($LASTEXITCODE -ne 0) {
  Write-Error "Failed to publish GeothermalResearchInstitute FakePlcV2 project."
}

dotnet.exe publish -c "Release" -f "netcoreapp3.1" -o (Join-Path $OutputRoot "GeothermalResearchInstitute/Wpf") (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/GeothermalResearchInstitute.Wpf")
if ($LASTEXITCODE -ne 0) {
  Write-Error "Failed to publish GeothermalResearchInstitute Wpf project."
}

Get-ChildItem -Path (Join-Path $OutputRoot "GeothermalResearchInstitute") -Filter "*.ini" -Recurse `
  | ForEach-Object{ & "$UnixToDosExec" $_.FullName }

New-Item -Path (Join-Path $OutputRoot "GeothermalResearchInstitute/docs") -Type Directory
Copy-Item -Path (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/docs/*") -Destination (Join-Path $OutputRoot "GeothermalResearchInstitute/docs") -Include "*.pdf", "*.exe", "*.msu" -Recurse
