$ErrorActionPreference = "Stop"

Write-Host "Running unit and integration tests..." -ForegroundColor Cyan

dotnet test AuthApi.UnitTests\AuthApi.UnitTests.csproj --no-restore
dotnet test AuthApi.IntegrationTests\AuthApi.IntegrationTests.csproj --no-restore
dotnet test PetApi.IntegrationTests\PetApi.IntegrationTests.csproj --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Tests failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "All unit and integration tests passed." -ForegroundColor Green