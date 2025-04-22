# This script is cross-platform, supporting all OSes that PowerShell Core/7 runs on.

$currentDirectory = Get-Location
Write-Host "Current Directory is $currentDirectory `n"
$rootDirectory = git rev-parse --show-toplevel
Write-Host "Root Directory is $rootDirectory `n"
$hostDirectory = Join-Path -Path $rootDirectory -ChildPath 'Identity.UI.Client'
Write-Host "Host Directory is $hostDirectory `n"
$infrastructurePrj = Join-Path -Path $rootDirectory -ChildPath 'Identity.UI.Infrastructure/Identity.UI.Infrastructure.csproj'
Write-Host "infrastructurePrj Directory is $infrastructurePrj `n"

Write-Host "Make sure you have run the WebAPI project. `n"

Set-Location -Path $hostDirectory
Write-Host "Host Directory is $hostDirectory `n"

Write-Host "Press any key to continue... `n"
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');

<# Run command #>
dotnet build -t:NSwag $infrastructurePrj

Set-Location -Path $currentDirectory
Write-Host -NoNewLine 'NSwag Regenerated. Press any key to continue...';
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
