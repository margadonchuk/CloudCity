# Скрипт для восстановления изображений
# PowerShell скрипт для проверки и восстановления файлов

Write-Host "Проверка файлов изображений..." -ForegroundColor Yellow

$imagesPath = "CloudCityCenter/wwwroot/images"
$imageFiles = Get-ChildItem -Path $imagesPath -Filter "*.png" -ErrorAction SilentlyContinue

if ($imageFiles) {
    Write-Host "Найдено файлов: $($imageFiles.Count)" -ForegroundColor Green
    Write-Host "Файлы присутствуют локально!" -ForegroundColor Green
    
    Write-Host "`nСписок файлов:" -ForegroundColor Cyan
    $imageFiles | ForEach-Object { Write-Host "  - $($_.Name)" }
    
    Write-Host "`nЧтобы восстановить файлы на сервере:" -ForegroundColor Yellow
    Write-Host "1. Закоммитьте файлы: git add $imagesPath && git commit -m 'Restore images' && git push" -ForegroundColor White
    Write-Host "2. На сервере: git pull" -ForegroundColor White
    Write-Host "3. Или скопируйте через SCP/WinSCP" -ForegroundColor White
} else {
    Write-Host "Файлы не найдены! Восстановление из git..." -ForegroundColor Red
    
    # Попытка восстановить из git
    Write-Host "Восстановление из git..." -ForegroundColor Yellow
    git checkout HEAD -- $imagesPath
    
    $restoredFiles = Get-ChildItem -Path $imagesPath -Filter "*.png" -ErrorAction SilentlyContinue
    if ($restoredFiles) {
        Write-Host "Восстановлено файлов: $($restoredFiles.Count)" -ForegroundColor Green
    } else {
        Write-Host "Не удалось восстановить из git. Проверьте корзину Windows или бэкапы." -ForegroundColor Red
    }
}

Write-Host "`nГотово!" -ForegroundColor Green

