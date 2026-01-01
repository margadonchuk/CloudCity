# Восстановление изображений и информации на сайте

## Ситуация
Файлы изображений локально присутствуют в папке `CloudCityCenter/wwwroot/images/` (около 60 файлов).

## Если файлы были удалены на сервере Ubuntu:

### Вариант 1: Восстановить через Git (если файлы в репозитории)

На сервере Ubuntu выполните:

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter
git status wwwroot/images/
git checkout HEAD -- wwwroot/images/
```

### Вариант 2: Скопировать файлы с локальной машины на сервер

**Через SCP (с Windows PowerShell или Linux/Mac):**

```powershell
# В PowerShell на Windows:
scp -r "C:\Users\master1\Desktop\cloudcity\CloudCityCenter\CloudCity\CloudCityCenter\wwwroot\images\*" siteadmin@ваш_сервер:/home/siteadmin/cloudcity/CloudCityCenter/wwwroot/images/
```

**Или через Git:**

1. На локальной машине (Windows):
```powershell
cd C:\Users\master1\Desktop\cloudcity\CloudCityCenter\CloudCity
git add CloudCityCenter/wwwroot/images/
git commit -m "Restore images"
git push
```

2. На сервере Ubuntu:
```bash
cd /home/siteadmin/cloudcity
git pull
```

### Вариант 3: Использовать WinSCP или FileZilla

1. Скачайте WinSCP (https://winscp.net/) или используйте FileZilla
2. Подключитесь к серверу Ubuntu
3. Скопируйте все файлы из локальной папки:
   `C:\Users\master1\Desktop\cloudcity\CloudCityCenter\CloudCity\CloudCityCenter\wwwroot\images\`
   
   В папку на сервере:
   `/home/siteadmin/cloudcity/CloudCityCenter/wwwroot/images/`

## Список файлов для копирования (локально присутствуют):

- about.png, affordability.png, backend.png, Backup.png
- bank.png, banner.png, bitcoin.png, card.png, chr.png
- client1.png до client9.png (9 файлов)
- contact.png, database.png, ddos.png, dell.png
- digitalocean.png, email.png, ethereum.png, expertise.png
- finance.png, frontend.png, hetzner.png, Home1.png
- homeabout.png, hosting.png, hp.png, humanfirewall.png
- hyperv.png, logo.png, logotg.png, mikrotik.png
- ms.png, network.png, ourservices.png, ovh.png
- paypal.png, proxmox.png, reliability.png, security.png
- service1.png, service2.png, service3.png, storage.png
- support.png, telegram.png, tether.png, ubuntu.png
- vpn-device.png, vpn-network.png, vpnhero.png
- vps.png, vps1.png до vps6.png (7 файлов)
- webdev.png, webhosting.png, wg.png
- winserv.png, worldmap.png, zabbix.png

**Всего: ~60 файлов изображений**

## После копирования на сервер:

1. Проверьте права доступа:
```bash
sudo chown -R siteadmin:siteadmin /home/siteadmin/cloudcity/CloudCityCenter/wwwroot/images/
sudo chmod -R 644 /home/siteadmin/cloudcity/CloudCityCenter/wwwroot/images/*.png
```

2. Перезапустите приложение:
```bash
sudo systemctl restart cloudcity
```

3. Проверьте логи:
```bash
sudo journalctl -u cloudcity -f
```

## Если файлы были удалены локально:

### Восстановление из корзины Windows:

1. Откройте корзину Windows
2. Найдите папку `images` или отдельные файлы .png
3. Восстановите их обратно в:
   `C:\Users\master1\Desktop\cloudcity\CloudCityCenter\CloudCity\CloudCityCenter\wwwroot\images\`

### Восстановление через Git:

```powershell
cd C:\Users\master1\Desktop\cloudcity\CloudCityCenter\CloudCity
git status
git checkout HEAD -- CloudCityCenter/wwwroot/images/
```

Или восстановить из конкретного коммита:
```powershell
git log --all --full-history -- CloudCityCenter/wwwroot/images/
git checkout <commit-hash> -- CloudCityCenter/wwwroot/images/
```

## Проверка после восстановления:

1. Убедитесь, что файлы на месте:
```bash
# На Ubuntu:
ls -la /home/siteadmin/cloudcity/CloudCityCenter/wwwroot/images/*.png | wc -l
# Должно быть ~60 файлов
```

2. Проверьте сайт в браузере
3. Откройте DevTools (F12) → Network → обновите страницу
4. Проверьте, что запросы к изображениям возвращают 200 (не 404)

