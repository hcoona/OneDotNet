param(
  [Switch] $ChangedOnly
)

$ErrorActionPreference = "Stop"

$EnlistmentRoot = [System.IO.Path]::GetDirectoryName([System.IO.Path]::GetDirectoryName($PSScriptRoot))
$DosToUnixExec = Join-Path $EnlistmentRoot "dev-support/bin/dos2unix.exe"

Push-Location $EnlistmentRoot

if ($ChangedOnly) {
  git status -s `
    | ForEach-Object { & $DosToUnixExec ($_.Trim() -split "\s+")[1] }
} else {
  # Get-ChildItem $EnlistmentRoot `
  #   -Recurse `
  #   -Exclude (Join-Path $EnlistmentRoot "third_party"), (Join-Path $EnlistmentRoot "output") `
  #   -Include "*.adoc", `
  #     "*.ini", "*.json", "*.yml", `
  #     "*.html", "*.xaml", `
  #     "*.cs", "*.fs", "*.proto", `
  #     "*.props", "*.*proj", "*.sln" `
  #   | ForEach-Object { & $DosToUnixExec $_.FullName }
}
dotnet.exe format

Pop-Location
