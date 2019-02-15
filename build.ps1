Param (
  [Parameter(Mandatory)]
  [string]$version = $(throw "-version is required.")
)

nuget pack .\src\TestData\TestData.csproj -Properties version="$version"