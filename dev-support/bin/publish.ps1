$ErrorActionPreference = "Stop"

$EnlistmentRoot = [System.IO.Path]::GetDirectoryName([System.IO.Path]::GetDirectoryName($PSScriptRoot))

$OutputRoot = (Join-Path $EnlistmentRoot "output")
if ([System.IO.Directory]::Exists($OutputRoot)) {
  Remove-Item -Path $OutputRoot -Recurse -Force
}
New-Item -Path $OutputRoot -ItemType Directory

dotnet.exe publish -c "Release" -o (Join-Path $OutputRoot "GeothermalResearchInstitute/ServerConsole") (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/GeothermalResearchInstitute.ServerConsole")
if ($LASTEXITCODE -ne 0) {
  Write-Error "Failed to publish GeothermalResearchInstitute ServerConsole project."
}

dotnet.exe publish -c "Release" -o (Join-Path $OutputRoot "GeothermalResearchInstitute/Wpf") (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/GeothermalResearchInstitute.Wpf")
if ($LASTEXITCODE -ne 0) {
  Write-Error "Failed to publish GeothermalResearchInstitute Wpf project."
}

Copy-Item -Path (Join-Path $EnlistmentRoot "GeothermalResearchInstitute/docs/") -Destination (Join-Path $OutputRoot "GeothermalResearchInstitute/docs") -Filter "*.pdf" -Recurse
