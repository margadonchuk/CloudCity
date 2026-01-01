# Инструкция по копированию файлов на сервер

## Вариант 1: Использование SCP (если есть SSH доступ)

С локальной машины (Windows PowerShell или Git Bash):

```bash
# Скопируйте обновленные файлы на сервер
scp CloudCityCenter/Views/Home/_Hero.cshtml siteadmin@ВАШ_IP:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/
scp CloudCityCenter/Views/Home/_Welcome.cshtml siteadmin@ВАШ_IP:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/
scp CloudCityCenter/Views/Home/_About.cshtml siteadmin@ВАШ_IP:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/
scp CloudCityCenter/Views/Home/_Testimonials.cshtml siteadmin@ВАШ_IP:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/
scp CloudCityCenter/Views/Home/_Services.cshtml siteadmin@ВАШ_IP:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/
scp CloudCityCenter/Views/Home/_Partners.cshtml siteadmin@ВАШ_IP:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/
scp CloudCityCenter/wwwroot/css/site.css siteadmin@ВАШ_IP:/home/siteadmin/CloudCity/CloudCityCenter/wwwroot/css/
scp CloudCityCenter/wwwroot/js/site.js siteadmin@ВАШ_IP:/home/siteadmin/CloudCity/CloudCityCenter/wwwroot/js/
```

## Вариант 2: Создать архив и загрузить

На локальной машине:
```bash
# Создайте архив с обновленными файлами
tar -czf updated-files.tar.gz \
  CloudCityCenter/Views/Home/_Hero.cshtml \
  CloudCityCenter/Views/Home/_Welcome.cshtml \
  CloudCityCenter/Views/Home/_About.cshtml \
  CloudCityCenter/Views/Home/_Testimonials.cshtml \
  CloudCityCenter/Views/Home/_Services.cshtml \
  CloudCityCenter/Views/Home/_Partners.cshtml \
  CloudCityCenter/wwwroot/css/site.css \
  CloudCityCenter/wwwroot/js/site.js
```

Затем загрузите архив на сервер (через FTP, SCP, или веб-интерфейс) и распакуйте:
```bash
# На сервере
cd /home/siteadmin/CloudCity/CloudCityCenter
tar -xzf updated-files.tar.gz
```

## Вариант 3: Использовать Personal Access Token для GitHub

1. Создайте токен на GitHub:
   - Зайдите на https://github.com/settings/tokens
   - Нажмите "Generate new token (classic)"
   - Выберите права: `repo` (полный доступ к репозиториям)
   - Скопируйте токен

2. Используйте токен вместо пароля:
```bash
git push origin main
# Username: margadonchuk
# Password: ВСТАВЬТЕ_ВАШ_ТОКЕН_ЗДЕСЬ
```

Или настройте URL с токеном:
```bash
git remote set-url origin https://ВАШ_ТОКЕН@github.com/margadonchuk/cloudcity.git
git push origin main
```

## Вариант 4: Использовать SSH ключ (рекомендуется)

1. Сгенерируйте SSH ключ (если нет):
```bash
ssh-keygen -t ed25519 -C "your_email@example.com"
```

2. Добавьте публичный ключ в GitHub:
```bash
cat ~/.ssh/id_ed25519.pub
# Скопируйте вывод и добавьте в GitHub Settings → SSH and GPG keys
```

3. Измените remote URL на SSH:
```bash
git remote set-url origin git@github.com:margadonchuk/cloudcity.git
git push origin main
```

