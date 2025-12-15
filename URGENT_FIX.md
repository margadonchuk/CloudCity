# 🚨 СРОЧНОЕ ИСПРАВЛЕНИЕ - СЕРВИС НЕ ЗАПУСКАЕТСЯ

## Выполните на сервере (прямо сейчас):

```bash
cd /home/siteadmin/cloudcity
git pull
chmod +x fix-service.sh
./fix-service.sh
```

Этот скрипт:
- ✅ Проверит логи
- ✅ Исправит пути в файле сервиса
- ✅ Перезапустит сервис
- ✅ Проверит что все работает

---

## Или вручную (если скрипт не работает):

### 1. Посмотрите логи чтобы понять ошибку:
```bash
sudo journalctl -u cloudcity -n 50 --no-pager
```

### 2. Проверьте правильные пути:
```bash
# Где dotnet?
which dotnet

# Существует ли DLL?
ls -la /home/siteadmin/cloudcity/CloudCityCenter/bin/Release/net8.0/CloudCityCenter.dll
```

### 3. Обновите файл сервиса вручную:

```bash
sudo nano /etc/systemd/system/cloudcity.service
```

**ВАЖНО:** Замените пути на правильные:
- `/usr/bin/dotnet` → путь из `which dotnet`
- Убедитесь что путь к DLL правильный
- Убедитесь что User правильный (может быть `root` или `siteadmin`)

### 4. Перезапустите:
```bash
sudo systemctl daemon-reload
sudo systemctl enable cloudcity
sudo systemctl restart cloudcity
sudo systemctl status cloudcity
```

### 5. Если все еще не работает - запустите вручную для проверки:

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000"
export USE_REVERSE_PROXY=true
dotnet bin/Release/net8.0/CloudCityCenter.dll
```

Если вручную запускается - проблема в файле сервиса. Если не запускается - проблема в приложении (смотрите ошибки).

