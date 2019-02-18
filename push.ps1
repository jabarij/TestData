param (
  [string]$version,
  [Parameter(Mandatory)]
  [string]$apiKey,
  [string]$source = "https://api.nuget.org/v3/index.json"
)

function Verbose ([string]$msg) {
  Write-Host $msg
}

if ($version -eq $null -or $version -eq "") {
  $latestPackageName = (ls SoterDevelopment.TestData.* | Select Name | Sort-Object -Descending | Select -First 1).Name
  $latestPackageVersion = [regex]::match($latestPackageName,'SoterDevelopment\.TestData\.(?<version>\d+\.\d+\.\d+(\.\d+)?)\.nupkg').Groups["version"].Value
  $version = $latestPackageVersion
}

if ($version -eq $null -or $version -eq "") {
  throw "No package version specified."
}

nuget push "SoterDevelopment.TestData.$version.nupkg" -apiKey $apiKey -source $source