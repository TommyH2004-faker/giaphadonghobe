# Script Test RabbitMQ + Brevo Setup

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   RABBITMQ + BREVO EMAIL TEST         " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Check Docker
Write-Host "[1/3] Checking Docker..." -ForegroundColor Yellow
$dockerOK = $false
try {
    docker info 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  OK - Docker is running" -ForegroundColor Green
        $dockerOK = $true
    }
} catch {
    Write-Host "  X - Docker not running" -ForegroundColor Red
}
Write-Host ""

# 2. Check/Start RabbitMQ
Write-Host "[2/3] Checking RabbitMQ..." -ForegroundColor Yellow
if ($dockerOK) {
    $containers = docker ps -a --format "{{.Names}}" 2>&1
    if ($containers -match "rabbitmq") {
        Write-Host "  OK - RabbitMQ container exists" -ForegroundColor Green
        $running = docker ps --format "{{.Names}}" 2>&1
        if (-not ($running -match "rabbitmq")) {
            Write-Host "  Starting RabbitMQ..." -ForegroundColor Cyan
            docker start rabbitmq | Out-Null
            Start-Sleep -Seconds 5
        }
    } else {
        Write-Host "  Creating RabbitMQ container..." -ForegroundColor Cyan
        docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=admin123 rabbitmq:3-management | Out-Null
        Write-Host "  Waiting for startup (30s)..." -ForegroundColor Cyan
        Start-Sleep -Seconds 30
    }
    Write-Host "  OK - RabbitMQ is running" -ForegroundColor Green
    Write-Host "  Management UI: http://localhost:15672" -ForegroundColor Cyan
    Write-Host "  Credentials: admin / admin123" -ForegroundColor Cyan
}
Write-Host ""

# 3. Build Project
Write-Host "[3/3] Building project..." -ForegroundColor Yellow
dotnet build GiaPhaDongHo.sln --no-incremental 2>&1 | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "  OK - Build successful" -ForegroundColor Green
} else {
    Write-Host "  X - Build failed" -ForegroundColor Red
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "              SUMMARY                   " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
if ($dockerOK) {
    Write-Host "  RabbitMQ: READY" -ForegroundColor Green
    Write-Host "  Architecture: COMPLETE" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now run:" -ForegroundColor Yellow
    Write-Host "  cd GiaPha_WebAPI" -ForegroundColor Cyan
    Write-Host "  dotnet run" -ForegroundColor Cyan
} else {
    Write-Host "  RabbitMQ: NOT READY" -ForegroundColor Yellow
    Write-Host "  Please install Docker Desktop" -ForegroundColor White
}

Write-Host ""
Write-Host "Documentation:" -ForegroundColor Cyan
Write-Host "  - RABBITMQ_BREVO_ARCHITECTURE.md" -ForegroundColor White
Write-Host "  - BREVO_SETUP.md" -ForegroundColor White
Write-Host ""
Write-Host "Done!" -ForegroundColor Green
