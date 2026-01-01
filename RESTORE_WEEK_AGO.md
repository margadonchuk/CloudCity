# Восстановление состояния проекта неделю назад

## Дата восстановления: 25 декабря 2025 года, 20:18:30

### Коммит для восстановления
**Хэш коммита:** `6b753a9`  
**Дата:** 25 декабря 2025, 20:18:30 +0300  
**Сообщение:** "update menu"

### Что было изменено с того момента

С момента коммита `6b753a9` до текущего состояния было внесено множество изменений:

#### Основные изменения:
1. **Удалено множество файлов локализации** (.resx файлы)
2. **Удалены контроллеры:**
   - `VDIController.cs` (309 строк)
   - `HealthController.cs` (88 строк)
   - `LanguageController.cs` (27 строк)

3. **Удалены модели:**
   - `LoginViewModel.cs`
   - `VDIPlanVm.cs`
   - `ConfigurationServiceVm.cs`
   - `VPSPlanVm.cs`
   - `WindowsServerPageVm.cs` (частично)

4. **Удалены представления:**
   - `Views/VDI/Index.cshtml` (796 строк)
   - `Views/Account/Login.cshtml` (96 строк)
   - `Views/Home/_GlobalDataCenters.cshtml`
   - `Views/Home/_Payments.cshtml`

5. **Удалены изображения:**
   - Множество VDI изображений (vdi_*.png)
   - Изображения Windows Server (germ*.png, ind*.png, nether*.png, pl*.png, usa*.png)
   - Изображения VPS (vps_*.png)
   - Другие изображения продуктов

6. **Изменены файлы:**
   - `Program.cs` (упрощен, удалено 270+ строк)
   - `SeedData.cs` (упрощен, удалено 1694+ строк)
   - `site.css` (изменен стиль)
   - Множество контроллеров и представлений

7. **Добавлены новые файлы:**
   - `CHECK_DATABASE.md`
   - `DATABASE_SETUP.md`
   - `RESTORE_DATABASE.md`
   - `RESTORE_IMAGES.md`
   - `FIX_MISSING_IMAGES.md`
   - `UPDATE_IMAGE_URLS.sql`
   - Новые изображения VPN и VPS

### Статистика изменений
- **402 файла изменено**
- **2424 строки добавлено**
- **34589 строк удалено**

## Инструкция по восстановлению

### ⚠️ ВАЖНО: Перед восстановлением

1. **Сделайте резервную копию текущего состояния:**
   ```bash
   git branch backup-current-state
   git push origin backup-current-state
   ```

2. **Сделайте резервную копию базы данных** (если нужно сохранить данные)

### Вариант 1: Полное восстановление (Hard Reset)

**⚠️ ОПАСНО: Это удалит все незакоммиченные изменения!**

```bash
# Сохраните текущие изменения (если есть)
git stash

# Откатите к коммиту неделю назад
git reset --hard 6b753a9

# Если нужно отправить на сервер (ОПАСНО!)
git push --force origin <ваша-ветка>
```

### Вариант 2: Создание новой ветки с восстановлением (РЕКОМЕНДУЕТСЯ)

```bash
# Создайте новую ветку от коммита неделю назад
git checkout -b restore-week-ago 6b753a9

# Отправьте на сервер
git push origin restore-week-ago

# На сервере переключитесь на эту ветку
cd /home/siteadmin/cloudcity
git fetch
git checkout restore-week-ago
cd CloudCityCenter
dotnet build --configuration Release
sudo systemctl restart cloudcity
```

### Вариант 3: Восстановление отдельных файлов

Если нужно восстановить только определенные файлы:

```bash
# Восстановить конкретный файл
git checkout 6b753a9 -- путь/к/файлу

# Восстановить все файлы из определенной директории
git checkout 6b753a9 -- CloudCityCenter/Controllers/

# Восстановить все изображения
git checkout 6b753a9 -- CloudCityCenter/wwwroot/images/
```

### Восстановление базы данных

**Важно:** Откат кода НЕ восстановит данные в базе данных!

Если нужно восстановить данные базы данных:

1. **Если есть бэкап:**
   ```bash
   sqlcmd -S 10.151.10.8 -U sa -P "пароль" -Q "RESTORE DATABASE CloudCityDB FROM DISK = '/path/to/backup.bak' WITH REPLACE;"
   ```

2. **Если нет бэкапа:**
   - Придется заново добавить продукты через админ-панель
   - Или использовать seed данные (если они были в том коммите)

## Файлы, которые будут восстановлены

### Контроллеры:
- `VDIController.cs` (будет восстановлен)
- `HealthController.cs` (будет восстановлен)
- `LanguageController.cs` (будет восстановлен)

### Представления:
- `Views/VDI/Index.cshtml`
- `Views/Account/Login.cshtml`
- `Views/Home/_GlobalDataCenters.cshtml`
- `Views/Home/_Payments.cshtml`

### Изображения:
- Все VDI изображения
- Все Windows Server изображения
- Все старые VPS изображения

### Файлы локализации:
- Все .resx файлы для различных языков

### Конфигурация:
- Полная версия `Program.cs`
- Полная версия `SeedData.cs`

## После восстановления

1. Проверьте, что приложение запускается:
   ```bash
   dotnet build
   dotnet run
   ```

2. Проверьте базу данных:
   ```bash
   dotnet ef database update
   ```

3. Если нужно, запустите seed данные:
   ```bash
   dotnet run --project CloudCityCenter -- --seed
   ```

4. Перезапустите сервис:
   ```bash
   sudo systemctl restart cloudcity
   ```

## Откат восстановления

Если восстановление не подошло и нужно вернуться к текущему состоянию:

```bash
# Вернуться к текущей ветке
git checkout <текущая-ветка>

# Или вернуться к последнему коммиту
git checkout main
git pull
```

## Контакты и поддержка

Если возникли проблемы при восстановлении, проверьте:
- Логи приложения: `journalctl -u cloudcity -f`
- Логи базы данных
- Файлы конфигурации

