-- Обновление изображений для услуг настройки
-- Устанавливаем setupWinserv.png только для услуг настройки Windows Server
-- Для CHR, MikroTik, Fortinet используем их оригинальные изображения

-- Обновляем услуги настройки Windows Server (по slug)
UPDATE Products 
SET ImageUrl = '/images/setupWinserv.png'
WHERE Slug LIKE 'windows-server-setup-%';

-- Обновляем CHR услуги настройки
UPDATE Products 
SET ImageUrl = '/images/chr1.png'
WHERE Slug LIKE 'config-chr-%' 
   AND Name LIKE '%Basic%'
   AND (ImageUrl IS NULL OR ImageUrl = '/images/setupWinserv.png');

UPDATE Products 
SET ImageUrl = '/images/chr2.png'
WHERE Slug LIKE 'config-chr-%' 
   AND Name LIKE '%Standard%'
   AND (ImageUrl IS NULL OR ImageUrl = '/images/setupWinserv.png');

UPDATE Products 
SET ImageUrl = '/images/chr3.png'
WHERE Slug LIKE 'config-chr-%' 
   AND Name LIKE '%Pro%'
   AND (ImageUrl IS NULL OR ImageUrl = '/images/setupWinserv.png');

-- Обновляем MikroTik услуги настройки
UPDATE Products 
SET ImageUrl = '/images/mikrotik1.png'
WHERE Slug LIKE 'config-mikrotik-%' 
   AND Name LIKE '%Basic%'
   AND (ImageUrl IS NULL OR ImageUrl = '/images/setupWinserv.png');

UPDATE Products 
SET ImageUrl = '/images/mikrotik2.png'
WHERE Slug LIKE 'config-mikrotik-%' 
   AND Name LIKE '%Advanced%'
   AND (ImageUrl IS NULL OR ImageUrl = '/images/setupWinserv.png');

UPDATE Products 
SET ImageUrl = '/images/mikrotik3.png'
WHERE Slug LIKE 'config-mikrotik-%' 
   AND Name LIKE '%Pro%'
   AND (ImageUrl IS NULL OR ImageUrl = '/images/setupWinserv.png');

-- Обновляем Fortinet услуги настройки
UPDATE Products 
SET ImageUrl = '/images/fortinet1.png'
WHERE Slug LIKE 'config-fortinet-%' 
   AND Name LIKE '%Basic%'
   AND (ImageUrl IS NULL OR ImageUrl = '/images/setupWinserv.png');

UPDATE Products 
SET ImageUrl = '/images/fortinet2.png'
WHERE Slug LIKE 'config-fortinet-%' 
   AND Name LIKE '%Security%'
   AND (ImageUrl IS NULL OR ImageUrl = '/images/setupWinserv.png');

UPDATE Products 
SET ImageUrl = '/images/fortinet3.png'
WHERE Slug LIKE 'config-fortinet-%' 
   AND Name LIKE '%Enterprise%'
   AND (ImageUrl IS NULL OR ImageUrl = '/images/setupWinserv.png');

GO
