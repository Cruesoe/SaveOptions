[CmdletBinding()]
param([string]$RimWorldRoot)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
& dotnet run --project (Join-Path $repo 'Tests\SaveOptions.Tests.csproj') -c Release
if ($LASTEXITCODE -ne 0) { throw 'Save policy tests failed.' }
$keys = ([xml][IO.File]::ReadAllText((Join-Path $repo 'Languages\English\Keyed\SaveOptions.xml'))).LanguageData.ChildNodes | Where-Object NodeType -eq Element | ForEach-Object Name
foreach ($file in Get-ChildItem (Join-Path $repo 'Source') -Filter *.cs) {
  foreach ($match in [regex]::Matches([IO.File]::ReadAllText($file.FullName), '"(SaveOptions_[A-Za-z]+)"')) {
    if ($match.Groups[1].Value -notin $keys) { throw "Missing translation key: $($match.Groups[1].Value)" }
  }
}
'All referenced Save Options translation keys are present.'
