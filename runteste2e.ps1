$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Running Playwright E2E tests" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$exitCode = 0

try {
    docker compose `
        -f docker-compose.test.yml `
        up `
        --build `
        --exit-code-from playwright `
        --menu=false


    $exitCode = $LASTEXITCODE
}
finally {
    Write-Host ""
    Write-Host "Stopping test environment..." -ForegroundColor Cyan

    docker compose `
        -f docker-compose.test.yml `
        down `
        --volumes `
        --remove-orphans
}

if ($exitCode -ne 0) {
    Write-Host ""
    Write-Host "E2E tests FAILED." -ForegroundColor Red
    exit $exitCode
}

Write-Host ""
Write-Host "E2E tests PASSED." -ForegroundColor Green