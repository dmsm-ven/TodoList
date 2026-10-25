<#
.SYNOPSIS
    Пересобирает docker-образ API и (пере)запускает docker compose.

.DESCRIPTION
    1. Собирает образ painven/todo_api:latest из TodoListApp.API/Dockerfile
       (контекст сборки - корень репозитория, т.к. Dockerfile копирует Directory.*.props и TodoListApp.Core).
    2. Запускает/обновляет контейнеры из docker-compose.yml (api + db).
       Compose заметит новый образ и пересоздаст только контейнер api, база и ее данные (volume pgdata_todo) не затрагиваются.

    Перед запуском рядом с docker-compose.yml должны лежать api.env и db.env (в git их нет).

.PARAMETER NoBuild
    Не пересобирать образ, только запустить/обновить compose.

.PARAMETER NoCache
    Собирать образ без кэша слоев (если нужна полностью чистая сборка).

.PARAMETER Prune
    После сборки удалить "висячие" образы (старые версии painven/todo_api без тега).

.EXAMPLE
    .\build-and-run.ps1
    .\build-and-run.ps1 -NoCache -Prune
    .\build-and-run.ps1 -NoBuild
#>
[CmdletBinding()]
param(
    [switch]$NoBuild,
    [switch]$NoCache,
    [switch]$Prune
)

$ErrorActionPreference = 'Stop'

# Контекст сборки docker - корень репозитория (папка, где лежит этот скрипт)
Set-Location -Path $PSScriptRoot

# Простая функция (без CmdletBinding), чтобы аргументы вида -f / -t / -d уходили в docker как есть через $args
function Invoke-Docker {
    & docker @args
    if ($LASTEXITCODE -ne 0) {
        throw "docker $($args -join ' ') завершился с кодом $LASTEXITCODE"
    }
}

# --- Проверки окружения ---
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    throw 'docker не найден в PATH. Установите и запустите Docker Desktop.'
}

cmd /c "docker info >nul 2>&1"
if ($LASTEXITCODE -ne 0) {
    throw 'Docker daemon недоступен. Запустите Docker Desktop и повторите.'
}

foreach ($envFile in 'api.env', 'db.env') {
    if (-not (Test-Path -Path $envFile)) {
        throw "Не найден файл $envFile (нужен docker-compose.yml, в git его нет). Создайте его рядом с docker-compose.yml."
    }
}

# --- Сборка образов ---
if (-not $NoBuild) {
    Write-Host '==> Сборка образа painven/todo_api:latest' -ForegroundColor Cyan

    $apiBuildArgs = @('build', '-f', 'TodoListApp.API/Dockerfile', '-t', 'painven/todo_api:latest')
    if ($NoCache) { $apiBuildArgs += '--no-cache' }
    $apiBuildArgs += '.'
    Invoke-Docker @apiBuildArgs

    # WEB UI сейчас не пересобираем (проект отложен). Когда вернемся к нему - раскомментировать и уточнить путь к Dockerfile
    # (раньше был TodoListApp.WebUI/Dockerfile, образ painven/todo_web) и добавить сервис webui в docker-compose.yml:
    # Invoke-Docker build -f TodoListApp.WebUI/Dockerfile -t painven/todo_web:latest .

    if ($Prune) {
        Write-Host '==> Удаление висячих образов' -ForegroundColor Cyan
        Invoke-Docker image prune -f
    }
}

# --- Запуск compose ---
Write-Host '==> docker compose up -d' -ForegroundColor Cyan
Invoke-Docker compose up -d

Write-Host ''
Invoke-Docker compose ps

Write-Host ''
Write-Host 'Готово. API: http://localhost:8090' -ForegroundColor Green
Write-Host 'Логи API: docker compose logs -f api'
