# 🔍 РУЧНАЯ ДИАГНОСТИКА - Выполните команды по порядку

Выполните эти команды на сервере и посмотрите результаты:

## 1. Проверка сервиса

```bash
sudo systemctl status cloudcity
```

## 2. Логи сервиса

```bash
sudo journalctl -u cloudcity -n 50 --no-pager
```

## 3. Проверка процесса приложения

```bash
ps aux | grep dotnet
```

## 4. Проверка порта 5000

```bash
netstat -tlnp | grep 5000
# или
ss -tlnp | grep 5000
```

## 5. Тест приложения локально

```bash
curl http://localhost:5000
```

## 6. Проверка nginx

```bash
sudo systemctl status nginx
sudo nginx -t
```

## 7. Проверка nginx конфигурации

```bash
cat /etc/nginx/sites-enabled/cloudcitylife.com
```

## 8. Тест через домен

```bash
curl -k https://cloudcitylife.com
```

## 9. Логи nginx

```bash
sudo tail -20 /var/log/nginx/error.log
```

## 10. Проверка файлов приложения

```bash
ls -la /home/siteadmin/cloudcity/CloudCityCenter/bin/Release/net8.0/CloudCityCenter.dll
which dotnet
```

---

## После диагностики:

**Пришлите результаты этих команд**, особенно:
- Результат `sudo systemctl status cloudcity`
- Результат `sudo journalctl -u cloudcity -n 50 --no-pager`
- Результат `curl http://localhost:5000`

