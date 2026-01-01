# PowerShell скрипт для восстановления состояния проекта неделю назад
# Дата: 25 декабря 2025, коммит 6b753a9

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Восстановление состояния неделю назад" -ForegroundColor Cyan
Write-Host "Коммит: 6b753a9 (25 декабря 2025, 20:18)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Проверка наличия git
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Host "ОШИБКА: Git не установлен!" -ForegroundColor Red
    exit 1
}

# Проверка, что мы в git репозитории
if (-not (Test-Path .git)) {
    Write-Host "ОШИБКА: Это не git репозиторий!" -ForegroundColor Red
    exit 1
}

# Показываем текущее состояние
Write-Host "Текущая ветка:" -ForegroundColor Yellow
git branch --show-current
Write-Host ""

# Показываем незакоммиченные изменения
$status = git status --porcelain
if ($status) {
    Write-Host "ВНИМАНИЕ: Есть незакоммиченные изменения!" -ForegroundColor Yellow
    Write-Host "Файлы с изменениями:" -ForegroundColor Yellow
    git status --short
    Write-Host ""
    
    $response = Read-Host "Сохранить изменения в stash? (y/n)"
    if ($response -eq "y" -or $response -eq "Y") {
        git stash push -m "Сохранено перед восстановлением неделю назад"
        Write-Host "Изменения сохранены в stash" -ForegroundColor Green
    } else {
        Write-Host "Продолжение без сохранения изменений..." -ForegroundColor Yellow
    }
    Write-Host ""
}

# Показываем информацию о коммите
Write-Host "Информация о коммите для восстановления:" -ForegroundColor Cyan
git log -1 6b753a9 --format="%h - %an, %ad : %s" --date=iso
Write-Host ""

# Показываем, что будет изменено
Write-Host "Проверка различий..." -ForegroundColor Yellow
$diffCount = (git diff --stat 6b753a9 HEAD | Measure-Object -Line).Lines
Write-Host "Найдено различий в $diffCount строках" -ForegroundColor Yellow
Write-Host ""

# Запрашиваем подтверждение
Write-Host "Выберите вариант восстановления:" -ForegroundColor Cyan
Write-Host "1. Создать новую ветку 'restore-week-ago' (БЕЗОПАСНО)" -ForegroundColor White
Write-Host "2. Hard reset текущей ветки (ОПАСНО - потеряете изменения!)" -ForegroundColor Red
Write-Host "3. Восстановить только изображения" -ForegroundColor White
Write-Host "4. Восстановить только контроллеры" -ForegroundColor White
Write-Host "5. Восстановить только представления" -ForegroundColor White
Write-Host "6. Отмена" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Ваш выбор (1-6)"

switch ($choice) {
    "1" {
        Write-Host "Создание новой ветки..." -ForegroundColor Yellow
        git checkout -b restore-week-ago 6b753a9
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Ветка 'restore-week-ago' создана успешно!" -ForegroundColor Green
            Write-Host "Для отправки на сервер выполните: git push origin restore-week-ago" -ForegroundColor Cyan
        } else {
            Write-Host "ОШИБКА при создании ветки!" -ForegroundColor Red
        }
    }
    "2" {
        Write-Host "ВНИМАНИЕ: Это удалит все незакоммиченные изменения!" -ForegroundColor Red
        $confirm = Read-Host "Вы уверены? (yes/no)"
        if ($confirm -eq "yes") {
            Write-Host "Выполнение hard reset..." -ForegroundColor Yellow
            git reset --hard 6b753a9
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Восстановление выполнено!" -ForegroundColor Green
                Write-Host "ВНИМАНИЕ: Для отправки на сервер потребуется force push!" -ForegroundColor Red
            } else {
                Write-Host "ОШИБКА при восстановлении!" -ForegroundColor Red
            }
        } else {
            Write-Host "Отменено" -ForegroundColor Yellow
        }
    }
    "3" {
        Write-Host "Восстановление изображений..." -ForegroundColor Yellow
        git checkout 6b753a9 -- CloudCityCenter/wwwroot/images/
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Изображения восстановлены!" -ForegroundColor Green
            Write-Host "Не забудьте закоммитить изменения: git add CloudCityCenter/wwwroot/images/ && git commit -m 'Restore images from week ago'" -ForegroundColor Cyan
        } else {
            Write-Host "ОШИБКА при восстановлении изображений!" -ForegroundColor Red
        }
    }
    "4" {
        Write-Host "Восстановление контроллеров..." -ForegroundColor Yellow
        git checkout 6b753a9 -- CloudCityCenter/Controllers/
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Контроллеры восстановлены!" -ForegroundColor Green
            Write-Host "Не забудьте закоммитить изменения" -ForegroundColor Cyan
        } else {
            Write-Host "ОШИБКА при восстановлении контроллеров!" -ForegroundColor Red
        }
    }
    "5" {
        Write-Host "Восстановление представлений..." -ForegroundColor Yellow
        git checkout 6b753a9 -- CloudCityCenter/Views/
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Представления восстановлены!" -ForegroundColor Green
            Write-Host "Не забудьте закоммитить изменения" -ForegroundColor Cyan
        } else {
            Write-Host "ОШИБКА при восстановлении представлений!" -ForegroundColor Red
        }
    }
    "6" {
        Write-Host "Отменено" -ForegroundColor Yellow
    }
    default {
        Write-Host "Неверный выбор!" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Готово!" -ForegroundColor Green

