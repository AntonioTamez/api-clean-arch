# Script para reiniciar la API

Write-Host "Deteniendo procesos de dotnet en el puerto 5000..." -ForegroundColor Yellow

# Buscar y matar procesos en puerto 5000
$processes = Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -Unique

foreach ($pid in $processes) {
    Write-Host "Deteniendo proceso PID: $pid" -ForegroundColor Red
    Stop-Process -Id $pid -Force -ErrorAction SilentlyContinue
}

Start-Sleep -Seconds 2

Write-Host ""
Write-Host "Iniciando la API..." -ForegroundColor Green
Write-Host ""

cd c:\ATS\GIT\api-clean-arch\src\CleanArch.API
dotnet run
